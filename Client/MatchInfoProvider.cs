using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

public class MatchInfoProvider : IMatchInfoDatastore
{
        private S3JsonReader _reader;
        private S3JsonWriter _writer;
        private static readonly string KEY_PATH_FORMAT = "{0}/{1}/{2}/{3}";
        private static readonly string CURRENT_KEY_FORMAT = KEY_PATH_FORMAT + "/" + "MatchInfo";
        private static readonly string CUP_KEY_FORMAT = "{0}/cup/{1}/{2}" + "/" + "MatchInfo";

        private int _leagueId;
        private ICollection<MatchInfo> _matchInfoCache = new List<MatchInfo>();

    public MatchInfoProvider(int leagueId, ICollection<MatchInfo> matchInfos) {
        init(leagueId, matchInfos);
    }

    public async Task<MatchInfo> readCurrent(int teamId, int eventId) {
        return await readInfo(createCurrentKey(teamId, eventId));
    }

    public async Task writeCurrent(int teamId, MatchInfo info) {
        await _writer.write(createCurrentKey(teamId, info.gameweek), info, true);
    }

    public async Task writeCup(int teamId, MatchInfo cup) {
        await _writer.write(createCupKey(teamId, cup.gameweek), cup, true);
    }

    public async Task delete(int teamId, int eventId) {
        await _writer.delete(createCurrentKey(teamId, eventId));
    }

    public async Task<ICollection<MatchInfo>> readAll() {
        if (_matchInfoCache != null && _matchInfoCache.Count > 0) {
            return _matchInfoCache;
        }

        var keys = await _reader.getKeys(string.Format(GlobalConfig.DataRoot + "/{0}", _leagueId));
        var matchInfos = new List<MatchInfo>();
        foreach (var key in keys) {
            if (!key.EndsWith(GlobalConfig.CloudAppConfig.CurrentGameWeek + "/MatchInfo")) {
                continue;
            }

            var info = await readInfo(key);
            if (info != null) {
                matchInfos.Add(info);
            }
        }
        return matchInfos;
    }

    private void init(int leagueId, ICollection<MatchInfo> matchInfos) {
        _reader = new S3JsonReader();
        _writer = new S3JsonWriter();
        _leagueId = leagueId;
        _matchInfoCache = matchInfos ?? _matchInfoCache;
    }

    private async Task<MatchInfo> readInfo(string keyName) {
        return await _reader.Read<MatchInfo>(keyName);
    }

    private string createCurrentKey(int teamId, int eventId) {
        return string.Format(CURRENT_KEY_FORMAT, GlobalConfig.MatchInfoRoot, _leagueId, teamId, eventId);
    }

    private string createCupKey(int teamId, int eventId) {
        return string.Format(CUP_KEY_FORMAT, GlobalConfig.MatchInfoRoot, teamId, eventId);
    }
}
