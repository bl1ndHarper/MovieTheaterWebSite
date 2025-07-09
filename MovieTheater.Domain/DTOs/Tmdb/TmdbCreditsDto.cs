    public class TmdbCreditsDto
    {
        public List<TmdbCastDto> Cast { get; set; } = new();
        public List<TmdbCrewDto> Crew { get; set; } = new();
    }

    public class TmdbCastDto
    {
        public string Name { get; set; }
    }

    public class TmdbCrewDto
    {
        public string Name { get; set; }
        public string Job { get; set; }
    }