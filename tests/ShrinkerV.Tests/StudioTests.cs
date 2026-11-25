using ShrinkerV.Core;
using ShrinkerV.Studio;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using CodeWalker.GameFiles;

internal static class StudioTests
{
    static int passed;
    static void Check(bool condition, string name) { if (!condition) throw new Exception(name); Console.WriteLine("PASS " + name); passed++; }
    static bool Skip(GtaProcessor proc, string path) { return (bool)typeof(GtaProcessor).GetMethod("ShouldSkipEntry", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(proc, new object[] { path }); }
    [STAThread]
    static int Main() {

        try { return Run(); } catch(Exception e) { Console.WriteLine(e); return 1; }
    }
    static int Run() {

        // DDS formats must never be silently reinterpreted under their old header.
        var ddsHeader = new byte[148];
        Action<int,int> put = (offset,value)=>Buffer.BlockCopy(BitConverter.GetBytes(value),0,ddsHeader,offset,4);
        put(0,0x20534444); put(4,124); put(12,64); put(16,64); put(76,32);
        put(80,0x40); put(88,16);
        Check(DdsEngine.ParseHeader(ddsHeader).Format == DdsEngine.DdsFmt.Unknown,"RGB16 is preserved instead of encoded as RGBA8");
        put(80,4); put(84,0x30315844); put(128,74);
        Check(DdsEngine.ParseHeader(ddsHeader).Format == DdsEngine.DdsFmt.Bc2,"DX10 BC2 retains its own codec");
        put(84,0x58585858);
        Check(DdsEngine.ParseHeader(ddsHeader).Format == DdsEngine.DdsFmt.Unknown,"unknown FourCC is not guessed as BC3");
        bool unsupportedBc7=false;
        try { DdsEngine.CompressRgba(new byte[64],4,4,DdsEngine.DdsFmt.Bc7); } catch(NotSupportedException) { unsupportedBc7=true; }
        Check(unsupportedBc7,"BC7 cannot silently emit BC3 blocks");
        foreach(var path in new[]{"x64v.rpf/models/cdimages/ped_mp_overlay_txds.rpf/mp_fm_body_hair_000.ytd","ped_sp_overlay_txds.rpf/skin.ytd","strm_peds_mp_overlay_txds.rpf/overlay.ytd","mp_fm_body_flan_001.ytd"}) Check(GtaProcessor.IsPlayerResource(path),"installed game player overlay: "+path);
        using (var form = new MainForm()) {
            var fields = typeof(MainForm);
            Func<string, System.Windows.Forms.CheckBox> toggle = name => (System.Windows.Forms.CheckBox)fields.GetField(name,BindingFlags.Instance|BindingFlags.NonPublic).GetValue(form);
            Check(toggle("_protectCharacters").Checked && !toggle("_clothingF2").Enabled,"UI protection default and disabled clothing controls");
            Check(toggle("_pedsF8").Enabled && toggle("_compressPeds").Checked,"UI allows NPC compression while player is protected");
            toggle("_compressPeds").Checked = false;
            Check(!toggle("_pedsF8").Enabled && toggle("_protectCharacters").Checked,"UI NPC preservation independent from player protection");
            toggle("_compressPeds").Checked = true;
            toggle("_protectCharacters").Checked = false;
            Check(toggle("_clothingF2").Enabled && toggle("_pedsF2").Enabled,"UI unlocks character settings");
            fields.GetMethod("ApplyPreset",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(form,new object[]{"Сильный"});
            Check(toggle("_f32").Checked && !toggle("_f8").Checked && toggle("_protectCharacters").Checked,"UI strong preset and protection");
            fields.GetMethod("ApplyPreset",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(form,new object[]{"Бережный"});
            Check(toggle("_f2").Checked && !toggle("_f32").Checked && !toggle("_removeMap").Checked,"UI gentle preset");
        }
        foreach (string p in new[] { "x64v.rpf/models/cdimages/streamedpeds_players.rpf/player_zero/head_diff_000_a.ytd", "dlc.rpf/x64/models/mp_m_freemode_01/jbib_000_u.ydd", "MP_F_FREEMODE_01.RPF/skin.ytd", "pedprops.rpf/p_head_000.ytd", "tattoo_overlays.ytd", "csb_abigail.ytd", "ig_michael/hair.ytd", "head_diff_000_a", "uppr_diff_001_a", "player_two.yft" }) Check(GtaProcessor.IsCharacterResource(p), "character " + p);
        foreach (string p in new[] { "x64j.rpf/levels/gta5/city/road.ytd", "mpbeach/vehicles.rpf/bifta.ytd", "mpsum/props.rpf/prop_table.ytd", "building_diff_01", "weapon_pistol.ytd" }) Check(!GtaProcessor.IsCharacterResource(p), "environment " + p);
        var opts = new ProcessingOptions { ProtectCharacters = true, KeepAudio = true, KeepVideo = true, KeepEffects = true };
        var proc = new GtaProcessor(opts, null);
        foreach (var npcPath in new[] { "peds.rpf/a_m_m_business_01.ytd", "componentpeds.rpf/csb_abigail.ydd", "streamedpeds.rpf/ig_lester/head_diff_001.ytd", "pedprops.rpf/s_m_m_security_01/p_head_000.ytd" })
            Check(!Skip(proc,npcPath) && GtaProcessor.IsNpcResource(npcPath),"NPC processed with player protection: "+npcPath);
        foreach (var mixedPath in new[] { "peds.rpf", "componentpeds.rpf", "streamedpeds.rpf", "pedprops.rpf" })
            Check(!Skip(proc,mixedPath),"mixed container still traversed: "+mixedPath);
        foreach (var playerPath in new[] { "streamedpeds_players.rpf", "player_one/uppr_diff_001.ytd", "player_two.yft", "mp_f_freemode_01.rpf/hair.ytd", "ig_michael/head_diff.ytd", "cs_trevor.ydd", "pedprops.rpf/p_head_000.ytd" })
            Check(Skip(proc,playerPath),"player preserved: "+playerPath);
        var protectTexture = typeof(GtaProcessor).GetMethod("ShouldProtectPlayerTexture",BindingFlags.NonPublic|BindingFlags.Instance);
        Check(!(bool)protectTexture.Invoke(proc,new object[]{"componentpeds.rpf/a_m_m_business_01.ytd","head_diff_000_a"}),"NPC skin name does not trigger player protection");
        Check((bool)protectTexture.Invoke(proc,new object[]{"mp_m_freemode_01.rpf/shared.ytd","diffuse"}),"MP ownership protects even generic texture names");
        opts.SkipPeds=true; Check(Skip(proc,"ig_lester.ytd"),"optional NPC preservation"); opts.SkipPeds=false;
        Check(Skip(proc,"mp_m_freemode_01/jbib.ydd"),"protected entry skipped");
        opts.ProtectCharacters = false; Check(!Skip(proc,"mp_m_freemode_01/jbib.ydd"),"protection can be disabled");
        foreach(string ext in new[]{"awc","bik","ypt"}) Check(Skip(proc,"content/test."+ext),"keep "+ext);
        opts.KeepAudio = opts.KeepVideo = opts.KeepEffects = false;
        foreach(string ext in new[]{"awc","bik","ypt"}) Check(!Skip(proc,"content/test."+ext),"process "+ext);
        opts.ProtectCharacters = true;
        var texture = new Texture { Name = "head_diff_000_a" };
        var processTex = typeof(GtaProcessor).GetMethod("ProcessTex", BindingFlags.NonPublic | BindingFlags.Instance);
        Check(!(bool)processTex.Invoke(proc,new object[]{texture,"generic.ytd"}) && proc.Stats.TexturesSkipped == 1,"skin in shared dictionary skipped before decoding");
        texture.Name = "vehicle_palette";
        Check(!(bool)processTex.Invoke(proc,new object[]{texture,"vehicle_palette.ytd"}),"grayscale disabled preserves palette");
        var cancel = new CancellationTokenSource(); cancel.Cancel(); opts.Cancel = cancel.Token;
        bool canceled = false; try { proc.Run(); } catch(OperationCanceledException) { canceled = true; }
        Check(canceled,"cancellation honored");
        string sandbox = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "shrinker-v-api-" + Guid.NewGuid().ToString("N"))).FullName;
        try {
            var input = Directory.CreateDirectory(Path.Combine(sandbox, "input")).FullName;
            var output = Directory.CreateDirectory(Path.Combine(sandbox, "output")).FullName;
            var apiOptions = Shrinker.CreateDefaultOptions();
            Check(apiOptions.ProtectCharacters && apiOptions.KeepAudio && apiOptions.KeepEffects && apiOptions.KeepVideo, "safe API defaults");
            apiOptions.InputFolder = input;
            apiOptions.OutputFolder = Path.Combine(input, "nested");
            bool rejected = false;
            try { Shrinker.Run(apiOptions); } catch (ArgumentException) { rejected = true; }
            Check(rejected && !Directory.Exists(apiOptions.OutputFolder), "nested output rejected before writing");
            apiOptions.OutputFolder = output;
            File.WriteAllText(Path.Combine(input, "bad.rpf"), "invalid archive");
            File.WriteAllText(Path.Combine(output, "bad.rpf"), "previous result");
            var failure = Shrinker.Run(apiOptions);
            Check(failure.ArchivesFailed == 1 && File.ReadAllText(Path.Combine(output, "bad.rpf")) == "previous result", "failed run preserves prior output");
        } finally { Directory.Delete(sandbox, true); }
        if (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("SHRINKER_V_TEST_INPUT"))) { Console.WriteLine("TOTAL PASS: "+passed+" (optional RPF fixture skipped)"); return 0; }
        string root = Path.Combine(Path.GetTempPath(),"shrinker-v-tests-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
        try {
        opts = new ProcessingOptions { InputFolder = Environment.GetEnvironmentVariable("SHRINKER_V_TEST_INPUT"), OutputFolder = Path.Combine(root,"smoke"), ProtectCharacters = true, KeepAudio = true, KeepVideo = true, KeepEffects = true, MaxThreads = 2, MinTextureSize = 32 };
        var before = File.ReadAllBytes(Path.Combine(opts.InputFolder,"x64a.rpf"));
        var stats = Shrinker.Run(opts,Console.WriteLine);
        Check(stats.ArchivesProcessed == 1 && stats.ErrorsEncountered == 0,"real RPF pipeline without errors");
        Check(stats.TexturesShrunken > 0,"textures actually compressed");
        Check(before.SequenceEqual(File.ReadAllBytes(Path.Combine(opts.InputFolder,"x64a.rpf"))),"source unchanged");
        var output = new RpfFile(Path.Combine(opts.OutputFolder,"x64a.rpf"),"test.rpf"); output.ScanStructure(null,null);
        Check(output.AllEntries != null && output.AllEntries.Count > 0,"output archive reopens");
        string mixedInput = Path.Combine(root,"mixed_"+DateTime.UtcNow.Ticks); Directory.CreateDirectory(mixedInput);
        string fixtureName = "mixed_" + DateTime.UtcNow.Ticks + ".rpf";
        var fixture = RpfFile.CreateNew(mixedInput, fixtureName, RpfEncryption.OPEN);
        var child = RpfFile.CreateNew(fixture.Root,"mp_m_freemode_01.rpf",RpfEncryption.OPEN);
        byte[] payload = Enumerable.Range(0,4096).Select(i=>(byte)(i%251)).ToArray();
        RpfFile.CreateFile(child.Root,"skin.bin",payload,true);
        var sourceRpf = new RpfFile(Path.Combine(opts.InputFolder,"x64a.rpf"),"x64a.rpf"); sourceRpf.ScanStructure(null,null);
        byte[] texturePayload = null;
        foreach(var sourceEntry in sourceRpf.AllEntries.OfType<RpfFileEntry>().Where(e=>e.Name.EndsWith(".ytd"))) {
            var candidate = sourceRpf.ExtractFile(sourceEntry);
            var ytd = new YtdFile(sourceEntry); ytd.Load(candidate, sourceEntry);
            if(ytd.TextureDict != null && ytd.TextureDict.Textures.data_items.Any(t=>t != null && t.Width>=256 && t.Height>=256)) { texturePayload=ytd.Save(); break; }
        }
        Check(texturePayload!=null,"valid texture fixture found");
        RpfFile.CreateFile(child.Root,"head_diff_000_a.ytd",texturePayload,true);
        var mixedPedArchive = RpfFile.CreateNew(fixture.Root,"componentpeds.rpf",RpfEncryption.OPEN);
        var npcChild = RpfFile.CreateNew(mixedPedArchive.Root,"a_m_m_business_01.rpf",RpfEncryption.OPEN);
        RpfFile.CreateFile(npcChild.Root,"head_diff_000_a.ytd",texturePayload,true);
        var singlePlayer = RpfFile.CreateNew(mixedPedArchive.Root,"player_zero.rpf",RpfEncryption.OPEN);
        RpfFile.CreateFile(singlePlayer.Root,"head_diff_000_a.ytd",texturePayload,true);
        RpfFile.CreateFile(fixture.Root,"intro.bik",payload,true);
        RpfFile.Defragment(fixture,null,true);
        var originalMpEntry=child.AllEntries.OfType<RpfFileEntry>().First(e=>e.Name.EndsWith(".ytd"));
        byte[] originalTextureBytes=child.ExtractFile(originalMpEntry);
        var mixedOptions = new ProcessingOptions { InputFolder=mixedInput,OutputFolder=Path.Combine(root,"mixed_output"),ProtectCharacters=true,KeepVideo=false,KeepEffects=true,KeepAudio=true,MaxThreads=1 };
        var mixedStats = Shrinker.Run(mixedOptions,Console.WriteLine);
        Check(mixedStats.ErrorsEncountered == 0 && mixedStats.ArchivesModified > 0,"mixed nested archive processed");
        var mixedResult = new RpfFile(Path.Combine(mixedOptions.OutputFolder,fixtureName),fixtureName); mixedResult.ScanStructure(null,null);
        var protectedChild = mixedResult.Children.First(c=>c.Name=="mp_m_freemode_01.rpf");
        var protectedEntry = protectedChild.AllEntries.OfType<RpfFileEntry>().First(e=>e.Name=="skin.bin");
        Check(payload.SequenceEqual(protectedChild.ExtractFile(protectedEntry)),"nested character payload byte-identical after parent defragmentation");
        var mpTexture = protectedChild.AllEntries.OfType<RpfFileEntry>().First(e=>e.Name.EndsWith(".ytd"));
        Check(originalTextureBytes.SequenceEqual(protectedChild.ExtractFile(mpTexture)),"MP textures byte-identical");
        var mixedPedResult = mixedResult.Children.First(c=>c.Name=="componentpeds.rpf");
        var playerResult = mixedPedResult.Children.First(c=>c.Name=="player_zero.rpf");
        Check(originalTextureBytes.SequenceEqual(playerResult.ExtractFile(playerResult.AllEntries.OfType<RpfFileEntry>().First())),"single player textures byte-identical inside mixed archive");
        var npcResult = mixedPedResult.Children.First(c=>c.Name=="a_m_m_business_01.rpf");
        Check(!originalTextureBytes.SequenceEqual(npcResult.ExtractFile(npcResult.AllEntries.OfType<RpfFileEntry>().First())) && mixedStats.TexturesShrunken>0,"NPC textures compressed inside same mixed archive");
        var videoEntry = mixedResult.AllEntries.OfType<RpfFileEntry>().First(e=>e.Name=="intro.bik");
        Check(!payload.SequenceEqual(mixedResult.ExtractFile(videoEntry)),"unprotected sibling actually modified");
        Console.WriteLine("TOTAL PASS: "+passed); return 0;
        } finally { Directory.Delete(root, true); }
    }
}
