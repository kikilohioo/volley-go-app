using System.Windows.Input;

namespace VolleyGo.Interfaces;

public interface IChampionshipFormViewModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxTeams { get; set; }

    public ICommand SubmitCommand { get; }
}
