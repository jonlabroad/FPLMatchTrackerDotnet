using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class HighlightCache
{
    private int _gameweek;

    public HighlightCache(int gameweek) {
        _gameweek = gameweek;
    }

    public async Task<bool> hasChanged(PlaylistItem[] newItems) {
        var previous = await getPreviousItems();
        if (previous != null && previous.Length == newItems.Length) {
            Console.WriteLine("Identical number of highlights in latest data and current cache");
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
            var file = new StreamWriter(getFilepath());
            Directory.CreateDirectory("cache");
            await file.WriteAsync(json);
            file.Close();
        } catch (Exception e) {
            Console.WriteLine(e);
        }
    }

    public async Task<PlaylistItem[]> getPreviousItems() {
        String itemData = "";
        try {
            itemData = await new StreamReader(getFilepath()).ReadToEndAsync();
            return JsonConvert.DeserializeObject<PlaylistItem[]>(itemData);
        } catch (Exception e) {
            Console.WriteLine("No existing highlight cache found at " + getFilepath());
            return null;
        }
    }

    private string getFilepath() {
        return string.Format("cache/highlight_{0}.json", _gameweek);
    }
}