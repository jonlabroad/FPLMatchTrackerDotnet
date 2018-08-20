using System.Threading.Tasks;

public interface IMatchInfoDatastore
{
    Task<MatchInfo> readCurrent(int teamId, int eventId);
    Task writeCurrent(int teamId, MatchInfo info);
}
