using SQLite;

namespace FMEditor.Database
{
    public class FmmDatabase
    {
        SQLiteAsyncConnection Database;

        public FmmDatabase()
        {
        }

        async Task Init()
        {
            if (Database is not null)
                return;

            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await Database.CreateTableAsync<EntityNation>();
        }

        public async Task<int> SaveItemAsync<T>(IEnumerable<T> items)
        {
            await Init();
            await Database.DeleteAllAsync<T>();
            return await Database.InsertAllAsync(items);
        }

        public async Task<List<EntityNation>> GetItemsAsync()
        {
            await Init();
            return await Database.Table<EntityNation>().ToListAsync();
        }

        //public async Task<List<Nation>> GetItemsNotDoneAsync()
        //{
        //    await Init();
        //    return await Database.Table<Nation>().Where(t => t.Done).ToListAsync();

        //    // SQL queries are also possible
        //    //return await Database.QueryAsync<TodoItem>("SELECT * FROM [TodoItem] WHERE [Done] = 0");
        //}

        //public async Task<Nation> GetItemAsync(int id)
        //{
        //    await Init();
        //    return await Database.Table<Nation>().Where(i => i.Id == id).FirstOrDefaultAsync();
        //}

        //public async Task<int> SaveItemAsync(Nation item)
        //{
        //    await Init();
        //    if (item.Id != 0)
        //        return await Database.UpdateAsync(item);
        //    else
        //        return await Database.InsertAsync(item);
        //}

        //public async Task<int> SaveItemAsync<T>(IEnumerable<T> items)
        //{
        //    await Init();
        //    await Database.DeleteAllAsync<T>();
        //    return await Database.InsertAllAsync(items);
        //}

        //public async Task<int> DeleteItemAsync(Nation item)
        //{
        //    await Init();
        //    return await Database.DeleteAsync(item);
        //}
    }
}
