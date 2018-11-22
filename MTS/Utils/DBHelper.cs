using System;
using System.IO;
using Android.Content;
using Android.Database.Sqlite;

namespace MTS.Utils
{
    public class DBHelper : SQLiteOpenHelper
    {
        private string dbPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        private string dbName;
        private static int version = 1;
        private Context context;

        public DBHelper(Context context, string dbName) : base(context, dbName, null, version)
        {
            this.context = context;
            this.dbName = dbName;
        }

        private string GetSQLiteDBPath()
        {
            return Path.Combine(dbPath, dbName);

        }

        public override SQLiteDatabase WritableDatabase
        {
            get
            {
                return CreateSQLiteDB();
            }
        }

        private SQLiteDatabase CreateSQLiteDB()
        {
            SQLiteDatabase sqliteDB = null;
            string path = GetSQLiteDBPath();
            Stream streamSQLite = null;
            FileStream streamWriter = null;
            Boolean isSQLiteInit = false;
            try
            {
                //if (File.Exists(path))
                //    isSQLiteInit = true;
                //else
                {
                    //streamSQLite = context.Resources.OpenRawResource(Resource.Raw.Busesn);
                    if (dbName == "BusDBN.db")
                    {
                        //streamSQLite = context.Resources.OpenRawResource(Resource.Raw.BusDBN);
                    }
                    if (dbName == "LoveDB.db")
                    {
                        //streamSQLite = context.Resources.OpenRawResource(Resource.Raw.LoveDB);
                    }
                    if (dbName == "TrollbusDB.db")
                    {
                        //streamSQLite = context.Resources.OpenRawResource(Resource.Raw.TrollbusDB);
                    }
                    streamWriter = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                    if (streamSQLite != null && streamWriter != null)
                    {
                        if (CopySQLiteDB(streamSQLite, streamWriter))
                        {
                            isSQLiteInit = true;
                        }
                    }
                }
                if (isSQLiteInit)
                {
                    sqliteDB = SQLiteDatabase.OpenDatabase(path, null, DatabaseOpenFlags.OpenReadwrite);

                }
            }
            catch
            {

            }
            return sqliteDB;
        }

        private bool CopySQLiteDB(Stream streamSQLite, FileStream streamWriter)
        {
            bool isSuccess = false;
            int length = 256;
            Byte[] buffer = new Byte[length];
            try
            {
                int bytesRead = streamSQLite.Read(buffer, 0, length);
                while (bytesRead > 0)
                {
                    streamWriter.Write(buffer, 0, bytesRead);
                    bytesRead = streamSQLite.Read(buffer, 0, length);
                }
                isSuccess = true;
            }
            catch
            {

            }
            finally
            {
                streamSQLite.Close();
                streamWriter.Close();
            }
            return isSuccess;
        }

        public void Add(string number, string type, Context c)
        {
            SQLiteDatabase db;
            DBHelper helper = new DBHelper(c, "LoveDB.db");
            db = helper.WritableDatabase;
            ContentValues cv = new ContentValues();
            cv.Put("0", "2");
            cv.Put("1", "bus");
            db.Insert("Love", "number", cv);
        }

        public override void OnCreate(SQLiteDatabase db)
        {
            throw new NotImplementedException();
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            throw new NotImplementedException();
        }
    }
}