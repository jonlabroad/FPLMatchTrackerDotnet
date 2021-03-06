using System;
using System.Collections.Generic;
using NLog;

public class ProcessedTeam
{
    public int id { get; set; }
    public List<ProcessedPick> picks { get; set; }
    public Score score { get; set; }
    public EntryData entry { get; set; }
    public TeamHistory history { get; set; }
    public string activeChip { get; set; }
    public List<TeamMatchEvent> events { get; set; }
    public List<TeamMatchEvent> autosubs { get; set; } = new List<TeamMatchEvent>();
    public int transferCost { get; set; }
    public List<string> form { get; set; }

    private static Logger _log = LogManager.GetCurrentClassLogger();
    public ProcessedTeam() {}

    public ProcessedTeam(int teamId, EntryData ent, List<ProcessedPick> processedPicks, Score s, List<TeamMatchEvent> eventList, string chip, TeamHistory hist, List<string> frm) {
        id = teamId;
        picks = processedPicks;
        score = s;
        events = eventList;
        entry = ent;
        activeChip = chip;
        history = hist;
        form = frm;
        if (!string.IsNullOrEmpty(activeChip)) {
            _log.Info(entry.entry.name + ": " + activeChip);
        }
    }

    public ProcessedPick getPick(int id) {
        foreach (var pick in picks) {
            if (pick.footballer.rawData.footballer.id == id) {
                return pick;
            }
        }
        return null;
    }

    public void setAutosubs(List<TeamMatchEvent> events) {
        autosubs = events;
    }
}
