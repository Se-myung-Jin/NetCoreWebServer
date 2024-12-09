namespace RankServer
{
    public partial class Redis
    {
        public static async Task<(long prevRanking, long nextRanking)> RankingUpdateScoreAsync(string key, int databaseIndex, long member, int score)
        {
            var db = connRedis.GetDatabase(databaseIndex);
            var trx = db.CreateTransaction();

            var prevRanking = trx.SortedSetRankAsync(key, member);
            _ = trx.SortedSetAddAsync(key, member, score);
            var nextRanking = trx.SortedSetRankAsync(key, member);

            await trx.ExecuteAsync();
            return (null == prevRanking.Result ? 0 : prevRanking.Result.Value + 1, null == nextRanking.Result ? 0 : nextRanking.Result.Value + 1);
        }
    }
}
