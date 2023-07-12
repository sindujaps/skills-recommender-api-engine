using skills_recommender_api.enums;


namespace skills_recommender_api
{
    public class RecommendationQuery
    {
        public int[] Sources { get; set; }
        public int NumberRecommendations { get; set; }
        public string Data { get; set; }
    }

}
