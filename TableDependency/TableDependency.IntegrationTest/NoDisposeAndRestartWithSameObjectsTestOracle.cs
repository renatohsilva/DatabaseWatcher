﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using TableDependency.Enums;
using TableDependency.EventArgs;
using TableDependency.IntegrationTest.Helpers.Oracle;
using TableDependency.Mappers;
using TableDependency.OracleClient;

namespace TableDependency.IntegrationTest
{
    public class TestOracleModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Qty { get; set; }
    }

    [TestClass]
    public class NoDisposeAndRestartWithSameObjectsTestOracle
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["OracleConnectionString"].ConnectionString;
        private static string TableName = "AAA_NODISPOSE_AND_REUSE".ToUpper();
        private static Dictionary<string, Tuple<TestOracleModel, TestOracleModel>> _checkValues = new Dictionary<string, Tuple<TestOracleModel, TestOracleModel>>();

        [ClassInitialize()]
        public static void ClassInitialize(TestContext testContext)
        {
            OracleHelper.DropTable(ConnectionString, TableName);

            using (var connection = new OracleConnection(ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"CREATE TABLE {TableName} (ID NUMBER(10), NAME VARCHAR2(50), \"Long Description\" VARCHAR2(4000))";
                    command.ExecuteNonQuery();
                }
            }
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            OracleHelper.DropTable(ConnectionString, TableName);
        }

        private void RunFirstTime(string namingToUse)
        {
            var mapper = new ModelToTableMapper<TestOracleModel>();
            mapper.AddMapping(c => c.Description, "Long Description").AddMapping(c => c.Name, "NAME");

            var tableDependency = new OracleTableDependency<TestOracleModel>(ConnectionString, TableName, mapper, false, namingToUse);
            tableDependency.OnChanged += TableDependency_Changed;
            tableDependency.Start(60, 120);
        }

        [TestCategory("Oracle")]
        [TestMethod]
        public void Test()
        {
            var namingToUse = "AAAMOSGTRO".ToUpper();

            var mapper = new ModelToTableMapper<TestOracleModel>();
            mapper.AddMapping(c => c.Description, "Long Description").AddMapping(c => c.Name, "NAME");

            RunFirstTime(namingToUse);
            Thread.Sleep(3 * 60 * 1010);

            using (var tableDependency = new OracleTableDependency<TestOracleModel>(ConnectionString, TableName, mapper, true, namingToUse))
            {
                tableDependency.OnChanged += TableDependency_Changed;
                tableDependency.Start(60, 120);
                Assert.AreEqual(tableDependency.DataBaseObjectsNamingConvention, namingToUse);

                Thread.Sleep(1 * 25 * 1000);

                var t = new Task(ModifyTableContent);
                t.Start();
                t.Wait(2 * 60 * 1000);
            }

            Assert.IsTrue(OracleHelper.AreAllDbObjectsDisposed(ConnectionString, namingToUse));
            Assert.AreEqual(_checkValues[ChangeType.Insert.ToString()].Item2.Name, _checkValues[ChangeType.Insert.ToString()].Item1.Name);
            Assert.AreEqual(_checkValues[ChangeType.Insert.ToString()].Item2.Description, _checkValues[ChangeType.Insert.ToString()].Item1.Description);
        }

        private static void TableDependency_Changed(object sender, RecordChangedEventArgs<TestOracleModel> e)
        {
            switch (e.ChangeType)
            {
                case ChangeType.Insert:
                    _checkValues[ChangeType.Insert.ToString()].Item2.Name = e.Entity.Name;
                    _checkValues[ChangeType.Insert.ToString()].Item2.Description = e.Entity.Description;
                    break;
            }
        }

        private static void ModifyTableContent()
        {
            _checkValues.Add(ChangeType.Insert.ToString(), new Tuple<TestOracleModel, TestOracleModel>(new TestOracleModel { Name = "Christian", Description = "Del Bianco" }, new TestOracleModel()));

            using (var connection = new OracleConnection(ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"BEGIN INSERT INTO {TableName} (ID, NAME, \"Long Description\") VALUES ({_checkValues[ChangeType.Insert.ToString()].Item1.Id}, '{_checkValues[ChangeType.Insert.ToString()].Item1.Name}', '{_checkValues[ChangeType.Insert.ToString()].Item1.Description}'); END;";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}