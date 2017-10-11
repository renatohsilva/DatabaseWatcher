﻿using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TableDependency.Enums;
using TableDependency.EventArgs;
using TableDependency.IntegrationTest.Helpers.SqlServer;
using TableDependency.Mappers;
using TableDependency.SqlClient;

namespace TableDependency.IntegrationTest
{
    [TestClass]
    public class ModelWithAnnotationUsedWithCunstructorParameterTestSqlServer
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["SqlServerConnectionString"].ConnectionString;
        private static readonly string TableName = "AAAA";
        private static int _counter = 0;
        private static readonly Dictionary<string, Item5> CheckValues = new Dictionary<string, Item5>();

        [ClassInitialize()]
        public static void ClassInitialize(TestContext testContext)
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = $"IF OBJECT_ID('{TableName}', 'U') IS NOT NULL DROP TABLE [{TableName}];";
                    sqlCommand.ExecuteNonQuery();

                    sqlCommand.CommandText = $"CREATE TABLE [{TableName}]([Id] [int] IDENTITY(1, 1) NOT NULL, [Name] [NVARCHAR](50) NULL, [More Info] [NVARCHAR](MAX) NULL)";
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        [TestInitialize()]
        public void TestInitialize()
        {
            CheckValues.Add(ChangeType.Insert.ToString(), new Item5());
            CheckValues.Add(ChangeType.Update.ToString(), new Item5());
            CheckValues.Add(ChangeType.Delete.ToString(), new Item5());
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = $"IF OBJECT_ID('{TableName}', 'U') IS NOT NULL DROP TABLE [{TableName}];";
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        [TestCategory("SqlServer")]
        [TestMethod]
        public void Test()
        {
            SqlTableDependency<Item5> tableDependency = null;
            string naming = null;

            var mapper = new ModelToTableMapper<Item5>();
            mapper.AddMapping(c => c.Infos, "More Info");

            var updateOf = new List<string>();
            updateOf.Add("More Info");

            try
            {
                tableDependency = new SqlTableDependency<Item5>(ConnectionString, TableName, mapper, updateOf);
                tableDependency.OnChanged += TableDependency_Changed;
                tableDependency.Start();
                naming = tableDependency.DataBaseObjectsNamingConvention;

                Thread.Sleep(5000);

                var t = new Task(ModifyTableContent);
                t.Start();
                t.Wait(30000);
            }
            finally
            {
                tableDependency?.Dispose();
            }

            Assert.AreEqual(_counter, 3);

            Assert.AreEqual(CheckValues[ChangeType.Insert.ToString()].Name, "Pizza MERGHERITA");
            Assert.AreEqual(CheckValues[ChangeType.Insert.ToString()].Infos, "Pizza MERGHERITA");

            Assert.AreEqual(CheckValues[ChangeType.Update.ToString()].Name, "Pizza MERGHERITA");
            Assert.AreEqual(CheckValues[ChangeType.Update.ToString()].Infos, "FUNGHI PORCINI");

            Assert.AreEqual(CheckValues[ChangeType.Delete.ToString()].Name, "Pizza");
            Assert.AreEqual(CheckValues[ChangeType.Delete.ToString()].Infos, "FUNGHI PORCINI");

            Assert.IsTrue(SqlServerHelper.AreAllDbObjectDisposed(ConnectionString, naming));
        }

        private static void TableDependency_Changed(object sender, RecordChangedEventArgs<Item5> e)
        {
            switch (e.ChangeType)
            {
                case ChangeType.Insert:
                    _counter++;                    
                    CheckValues[ChangeType.Insert.ToString()].Name = e.Entity.Name;
                    CheckValues[ChangeType.Insert.ToString()].Infos = e.Entity.Infos;
                    break;

                case ChangeType.Update:
                    _counter++;
                    CheckValues[ChangeType.Update.ToString()].Name = e.Entity.Name;
                    CheckValues[ChangeType.Update.ToString()].Infos = e.Entity.Infos;
                    break;

                case ChangeType.Delete:
                    _counter++;
                    CheckValues[ChangeType.Delete.ToString()].Name = e.Entity.Name;
                    CheckValues[ChangeType.Delete.ToString()].Infos = e.Entity.Infos;
                    break;
            }
        }

        private static void ModifyTableContent()
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = $"INSERT INTO [{TableName}] ([Name], [More Info]) VALUES ('Pizza MERGHERITA', 'Pizza MERGHERITA')";
                    sqlCommand.ExecuteNonQuery();
                    Thread.Sleep(500);

                    sqlCommand.CommandText = $"UPDATE [{TableName}] SET [More Info] = 'FUNGHI PORCINI'";
                    sqlCommand.ExecuteNonQuery();
                    Thread.Sleep(500);

                    sqlCommand.CommandText = $"UPDATE [{TableName}] SET [Name] = 'Pizza'";
                    sqlCommand.ExecuteNonQuery();
                    Thread.Sleep(500);

                    sqlCommand.CommandText = $"DELETE FROM [{TableName}]";
                    sqlCommand.ExecuteNonQuery();
                    Thread.Sleep(500);
                }
            }
        }
    }
}