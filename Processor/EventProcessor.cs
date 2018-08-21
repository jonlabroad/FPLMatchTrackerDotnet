using System.Threading.Tasks;

public class EventProcessor
{

    private EPLClient _client;
    private int _gameweek;

    public EventProcessor(EPLClient client, int gameweek)
    {
        _client = client;
        _gameweek = gameweek;
    }

    public async Task process()
    {
        await writeEventInfo();
    }

    private async Task writeEventInfo()
    {
        var liveData = await _client.getLiveData(_gameweek);
        var clubs = await _client.getClubs();

        var eventInfo = new EventInfo();
        eventInfo.eventId = _gameweek;
        eventInfo.fixtures = liveData.fixtures;
        eventInfo.clubs = clubs;
        await new S3JsonWriter().write(string.Format(GlobalConfig.DataRoot + "/events/{0}/EventInfo", _gameweek), eventInfo, true);
    }
}
