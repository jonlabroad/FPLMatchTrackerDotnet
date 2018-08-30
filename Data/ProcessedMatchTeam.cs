using System.Collections.Generic;

public class ProcessedMatchTeam : ProcessedTeam
{
    public Standing standing {get;set;}

    public ProcessedMatchTeam(ProcessedTeam baseTeam, Standing stand)
        : base(baseTeam.id, baseTeam.entry, baseTeam.picks, baseTeam.score, baseTeam.events, baseTeam.activeChip, baseTeam.history)
    {
        standing = stand;
    }

    public ProcessedMatchTeam(int teamId, EntryData ent, Standing stand, List<ProcessedPick> processedPicks, Score s, List<TeamMatchEvent> eventList, string chip, TeamHistory history)
        : base(teamId, ent, processedPicks, s, eventList, chip, history)
    {
        standing = stand;
    }
}
