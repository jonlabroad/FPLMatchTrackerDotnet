using System.Collections.Generic;

public class DifferentialFinder
{
    IList<ProcessedTeam> teams = new List<ProcessedTeam>();

    public DifferentialFinder(ProcessedTeam team1, ProcessedTeam team2) {
        teams.Add(team1);
        teams.Add(team2);
    }

    public HashSet<int> find() {
        HashSet<int> diff = new HashSet<int>();

        var captain = new List<HashSet<int>>();
        captain.Add(new HashSet<int>());
        captain.Add(new HashSet<int>());

        var starters = new List<HashSet<int>>();
        starters.Add(new HashSet<int>());
        starters.Add(new HashSet<int>());

        var subs = new List<HashSet<int>>();
        subs.Add(new HashSet<int>());
        subs.Add(new HashSet<int>());

        int teamNum = 0;
        foreach (var team in teams) {
            if (team == null || team.picks == null) {
                continue;
            }

            foreach (var pick in team.picks) {
                if (pick.isCaptain()) {
                    captain[teamNum].Add(pick.footballer.rawData.footballer.id);
                }
                else if (pick.pick.position <= 11) {
                    starters[teamNum].Add(pick.footballer.rawData.footballer.id);
                }
                else if (pick.pick.position > 11) {
                    subs[teamNum].Add(pick.footballer.rawData.footballer.id);
                }
            }
            teamNum++;
        }

        foreach (var val in getExclusion(captain[0], captain[1]))
        {
            diff.Add(val);
        }
        foreach (var val in getExclusion(starters[0], starters[1]))
        {
            diff.Add(val);
        }
        foreach (var val in getExclusion(subs[0], subs[1]))
        {
            diff.Add(val);
        }
        return diff;
    }

    protected HashSet<int> getExclusion(HashSet<int> set1, HashSet<int> set2) {
        HashSet<int> diff1 = new HashSet<int>(set1);
        HashSet<int> diff2 = new HashSet<int>(set2);
        foreach (var v in set2) {
            diff1.Remove(v);
        }
        foreach (var v in set2) {
            diff2.Remove(v);
        }

        var exclusion = new HashSet<int>();
        foreach (var v in diff1) {
            exclusion.Add(v);
        }
        foreach (var v in diff2) {
            exclusion.Add(v);
        }
        return exclusion;
    }
}
