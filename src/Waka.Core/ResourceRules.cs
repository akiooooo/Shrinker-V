using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using CodeWalker.GameFiles;
using CodeWalker.Utils;

namespace Waka.Core
{
    internal sealed partial class GtaProcessor
    {
        // ── SKIP RULES ────────────────────────────────────────────────────────────
        private bool ShouldSkipEntry(string path)
        {
            if (string.IsNullOrEmpty(path)) return true;
            string p = path.Replace('/', '\\').ToLowerInvariant();
            if (_opts.ProtectCharacters && IsPlayerResource(p)) return true;
            if (_opts.SkipPeds && IsNpcResource(p)) return true;
            if (_opts.KeepAudio && p.EndsWith(".awc")) return true;
            if (_opts.KeepVideo && p.EndsWith(".bik")) return true;
            if (_opts.KeepEffects && p.EndsWith(".ypt")) return true;
            if (_opts.SkipWeapons  && IsWeapon(p))   return true;
            if (IsWaterFog(p)) return true; // Keep shader data intact; vegetation is processed.
            return false;
        }

        public static bool IsProtectedByPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return true;
            string p = path.Replace('/', '\\').ToLowerInvariant();
            string n = System.IO.Path.GetFileName(p);
            string[] noTex = {
                "anim.rpf","cuts.rpf","audio_rel.rpf","data.rpf",
                "scaleform_generic.rpf","scaleform_platform_pc.rpf",
                "fonts.rpf","shaders.rpf","cloths.rpf","movie.rpf","web.rpf"
            };
            foreach (var b in noTex) if (n == b) return true;
            string[] noTexDirs = {
                "\\audio\\config\\","\\audio\\sfx\\","\\anim\\",
                "\\cutscene\\","\\scaleform\\","\\data\\lang\\","\\movies\\"
            };
            foreach (var b in noTexDirs) if (p.Contains(b)) return true;

            // ── МЕТАДАННЫЕ И СКРИПТЫ ──────────────────────────────────────────────
            // Здесь только то, где текстур физически нет: манифесты, XML-метаданные,
            // скрипты, датафайлы. Их изменение ломает загрузку DLC.
            string[] dlcCritical = {
                "\\common\\data",
                "\\levels\\gta5\\script","script_metadata",
                "\\metadata","content.xml","setup2.xml",
                "\\interiorproxies","interiorproxies",
                // GTA Enhanced Edition DLC — возможно другое шифрование.
                // Оставлено под защитой до подтверждения, что p9 не портит паки.
                "_g9ec\\","_g9ec/","\\mpg9ec\\","mpg9ec\\"
            };
            foreach (var b in dlcCritical) if (p.Contains(b)) return true;
            return false;
        }

        // Player ownership wins over NPC prefixes. Shared component names are protected
        // only when no explicit NPC model owns the file/dictionary.
        private static bool ExplicitPlayerPart(string part)
        {
            return part.Contains("ped_mp_overlay") || part.Contains("ped_sp_overlay") || part.Contains("peds_mp_overlay") || part.StartsWith("mp_fm_") || part.StartsWith("mp_head") || part.StartsWith("player_zero") || part.StartsWith("player_one") || part.StartsWith("player_two") ||
                part.StartsWith("mp_m_") || part.StartsWith("mp_f_") || part == "streamedpeds_players.rpf" ||
                part == "playerped.rpf" || part.StartsWith("ig_michael") || part.StartsWith("ig_franklin") || part.StartsWith("ig_trevor") ||
                part.StartsWith("cs_michael") || part.StartsWith("cs_franklin") || part.StartsWith("cs_trevor");
        }

        private static bool NpcPart(string part)
        {
            foreach (string prefix in new[] { "a_m_", "a_f_", "a_c_", "cs_", "csb_", "ig_", "s_m_", "s_f_", "u_m_", "u_f_", "g_m_", "g_f_", "hc_" })
                if (part.StartsWith(prefix)) return true;
            return false;
        }

        public static bool IsNpcResource(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            var parts = path.Replace('/', '\\').ToLowerInvariant().Split('\\');
            if (parts.Any(ExplicitPlayerPart)) return false;
            return parts.Any(NpcPart);
        }

        public static bool IsPlayerResource(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            var parts = path.Replace('/', '\\').ToLowerInvariant().Split('\\');
            if (parts.Any(ExplicitPlayerPart)) return true;
            if (parts.Any(NpcPart)) return false;
            // Generic peds/componentpeds/streamedpeds/pedprops containers can contain
            // both players and NPCs. Never exclude the whole mixed container.
            foreach (string part in parts) {
                foreach (string prefix in new[] { "head_", "hair_", "uppr_", "lowr_", "feet_", "hand_", "accs_", "task_", "jbib_", "berd_", "decl_", "teef_", "p_head", "p_eyes", "p_ears", "p_wrist", "p_lwrist", "p_rwrist", "skin_", "body_diff" })
                    if (part.StartsWith(prefix)) return true;
                if (part.StartsWith("tattoo_") || part.StartsWith("tat_") || part.Contains("pedhead") || part.Contains("pedoverlay") || part.Contains("clothing")) return true;
            }
            return false;
        }

        private bool ShouldProtectPlayerTexture(string container, string texture)
        {
            return _opts.ProtectCharacters && (IsPlayerResource(container) || (!IsNpcResource(container) && IsPlayerResource(texture)));
        }

        public static bool IsCharacterResource(string path) { return IsPlayerResource(path) || IsNpcResource(path); }
        private static bool IsClothing(string path) { return IsPlayerResource(path); }
        private static bool IsPed(string path) { return IsNpcResource(path); }

        private static bool IsWeapon(string p)
        {
            string[] pats = {
                "\\weapon","weapon_","weap_",
                "w_pi_","w_lr_","w_sb_","w_sr_",
                "w_mg_","w_lmg_","w_sg_","w_ar_","w_ex_","w_th_","w_am_",
                "w_hc_","w_gl_","w_fl_","w_rp_","w_snip"
            };
            foreach (var s in pats) if (p.Contains(s)) return true;
            return false;
        }

        // ── #1: ВОДА / ТУМАН — мерцание ───────────────────────────────────────────
        // waterfog и связанные текстуры читаются шейдером воды как градиент/маска
        // глубины. Уменьшение разрешения или ч/б ломает интерполяцию → мерцание.
        // Работает и по пути RPF, и по имени отдельной текстуры внутри YTD.
        private static bool IsWaterFog(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            string p = s.Replace('/', '\\').ToLowerInvariant();
            string[] pats = {
                "waterfog","water_fog","fogwater",
                "\\water\\","water.y",
                "waterheight","watertile","waterflow","watercaustic",
                "distantwater","oceanfoam","seafoam",
                "\\fog\\","fogvolume",
                "underwater","wetness","\\puddle","puddle_",
                "riverfoam","waves_","_waves","foam_","_foam"
            };
            foreach (var t in pats) if (p.Contains(t)) return true;
            return false;
        }
        // AWC stub: все .awc в \audio\sfx\ + все вне script.rpf
        private static bool IsLightweightAwc(RpfFileEntry entry, string topName)
        {
            if (entry.Name == null) return false;
            string lo = entry.Name.ToLowerInvariant();
            if (!lo.EndsWith(".awc")) return false;

            // Path-based: \audio\sfx\
            string ep = entry.Path != null ? entry.Path.Replace('/', '\\').ToLowerInvariant() : "";
            if (ep.Contains("\\audio\\sfx\\")) return true;

            // Fallback: все AWC вне script.rpf
            string tn = (topName ?? "").ToLowerInvariant();
            return !tn.Contains("script.rpf");
        }

        // YPT stub: все .ypt в \effects\ или \ptfx.rpf\
        private static bool IsLightweightYpt(RpfFileEntry entry)
        {
            if (entry.Name == null) return false;
            string lo = entry.Name.ToLowerInvariant();
            if (!lo.EndsWith(".ypt")) return false;

            string ep = entry.Path != null ? entry.Path.Replace('/', '\\').ToLowerInvariant() : "";
            if (ep.Contains("\\effects\\") || ep.Contains("\\ptfx.rpf\\")) return true;
            return false;
        }
        // HandleProcessCorruptedStateExceptions позволяет catch поймать
    }
}
