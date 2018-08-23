using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using static System.Net.Mime.MediaTypeNames;

public class HighlightCache
{
    private int _gameweek;
    private static Logger _log = LogManager.GetCurrentClassLogger();

    public HighlightCache(int gameweek) {
        _gameweek = gameweek;
    }

    public async Task<bool> hasChanged(PlaylistItem[] newItems) {
        var previous = await getPreviousItems();
        if (previous != null && previous.Length == newItems.Length) {
            _log.Info("Identical number of highlights in latest data and current cache");
            return false;
        }
        if (newItems != null) {
            await writeNewItems(newItems);
        }
        return true;
    }

    private async Task writeNewItems(PlaylistItem[] newItems) {
        String json = JsonConvert.SerializeObject(newItems);
        try {
            var filePath = getFilepath();
            var dirToCreate = Directory.GetCurrentDirectory() + "/cache";
            Directory.CreateDirectory(dirToCreate);
            var file = new StreamWriter(filePath);
            await file.WriteAsync(json);
            file.Close();
        } catch (Exception e) {
            _log.Error(e);
        }
    }

    public async Task<PlaylistItem[]> getPreviousItems() {
        String itemData = "";
        try {
            itemData = await new StreamReader(getFilepath()).ReadToEndAsync();
            return JsonConvert.DeserializeObject<PlaylistItem[]>(itemData);
        } catch (Exception) {
            _log.Error("No existing highlight cache found at " + getFilepath());
            return null;
        }
    }

    private string getFilepath() {
        return string.Format(Directory.GetCurrentDirectory() + "/cache/highlight_{0}.json", _gameweek);
    }
}
