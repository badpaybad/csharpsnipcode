 /*  1. Each PushEvent is worth 5 points.
     2. Each CreateEvent is worth 4 points.
     3. Each IssuesEvent is worth 3 points.
     4. Each CommitCommentEvent is worth 2 points.
     5. All other events are worth 1 point.*/

    public class GitHubScore
    {
        public class GithubEvent
        {
            public string id { get; set; }
            public string type { get; set; }
        }

        static HttpClient c;
        static GitHubScore()
        {
            c = new HttpClient();
            c.BaseAddress = new Uri("https://api.github.com/");
            c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
        }
     
        Dictionary<string, int> _eventScore = new Dictionary<string, int>();

        private string _gitHubUsername;
        public GitHubScore(string gitHubUsername)
        {
            _gitHubUsername = gitHubUsername;
            _eventScore["PushEvent"] = 5;
            _eventScore["CreateEvent"] = 4;
            _eventScore["IssuesEvent"] = 3;
            _eventScore["CommitCommentEvent"] = 2;
        }

        string GetEvents()
        {
            return c.GetAsync($"https://api.github.com/users/{_gitHubUsername}/events").GetAwaiter().GetResult().Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }

        public long GetScore()
        {
            string gitresponse = GetEvents();
            return JsonConvert.DeserializeObject<List<GithubEvent>>(gitresponse)
                .Select(i => _eventScore.TryGetValue(i.type, out int val) == true ? val : 1).Sum();
        }
    }
