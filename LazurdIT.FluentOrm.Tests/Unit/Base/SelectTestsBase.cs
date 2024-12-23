using System.Data.Common;
using FluentAssertions;
using LazurdIT.FluentOrm.Common;
using LazurdIT.FluentOrm.Tests.TestResources.Models;
using LazurdIT.FluentOrm.Tests.Utils;
using LazurdIT.FluentOrm.Tests.Utils.TestBase;

namespace LazurdIT.FluentOrm.Tests.Unit.Base;

public abstract class SelectTestsBase<TBase, TConnection, TRepository> where TBase : ITestBase<TConnection, TRepository> where TConnection : DbConnection, new() where TRepository : IFluentRepository<StudentModel>
{
    public abstract TBase TestBase { get; }

    private static void FillStudentList(TBase testBase, DbConnection connection)
    {
        var repository = testBase.NewStudentsRepository();
        var insertQuery = repository.Insert().WithFields(f => f.Exclude(f => f.Id));
        foreach (var student in SampleData.DefaultStudentsList)
        {
            //Adds a fix to pgsql test
            connection.Close();
            connection.Open();
            insertQuery.Execute(student, true, connection);
        }
    }

    [Fact(DisplayName = "Test select records")]
    public void TestSelectStudents()
    {
        var connectionString = TestBase.NewConnectionString();
        var repository = TestBase.NewStudentsRepository();
        TestBase.ToDoBefore(connectionString);

        using var connection = TestBase.NewConnection(connectionString);
        connection.Open();

        FillStudentList(TestBase, connection);

        var selectQuery = repository.Select();

        //Adds a fix to pgsql test
        connection.Close();
        connection.Open();
        var resultRecords = selectQuery.Execute(connection).ToList();

        resultRecords.Should().HaveCount(SampleData.DefaultStudentsList.Count, $"The record count should be {SampleData.DefaultStudentsList.Count}, but got {resultRecords.Count}");

        connection.Close();
    }

    [Fact(DisplayName = "Test select records with NotEq condition")]
    public void TestSelectStudentsWithNotEqCondition()
    {
        var connectionString = TestBase.NewConnectionString();
        var repository = TestBase.NewStudentsRepository();
        TestBase.ToDoBefore(connectionString);

        using var connection = TestBase.NewConnection(connectionString);
        connection.Open();

        FillStudentList(TestBase, connection);

        var selectQuery = repository.Select()
                .Where(wm => wm.NotEq(f => f.Id, 2));

        //Adds a fix to pgsql test
        connection.Close();
        connection.Open();
        var resultRecords = selectQuery.Execute(connection).ToList();

        resultRecords.Should().HaveCount(SampleData.DefaultStudentsList.Count - 1, $"The record count should be {SampleData.DefaultStudentsList.Count - 1}, but got {resultRecords.Count}");

        connection.Close();
    }

    [Fact(DisplayName = "Test select records with NotIn and nullables condition")]
    public void TestSelectStudentsWithNotInAndNullCondition()
    {
        var connectionString = TestBase.NewConnectionString();
        var repository = TestBase.NewStudentsRepository();
        TestBase.ToDoBefore(connectionString);

        using var connection = TestBase.NewConnection(connectionString);
        connection.Open();

        FillStudentList(TestBase, connection);
        var countWithoudData = SampleData.DefaultStudentsList.Count(x => x.Age != 15 && x.Age != 17);
        var selectQuery = repository.Select()
                                    .Where(wm => wm.Or(o => o.NotIn(f => f.Age, 15, 17)
                                                             .IsNull(f => f.Age)
                                                      )
                                    );

        //Adds a fix to pgsql test
        connection.Close();
        connection.Open();
        var resultRecords = selectQuery.Execute(connection).ToList();

        resultRecords.Should().HaveCount(countWithoudData, $"The record count should be {countWithoudData}, but got {resultRecords.Count}");

        connection.Close();
    }

    [Fact(DisplayName = "Test select records page")]
    public void TestSelectStudentsPage()
    {
        var connectionString = TestBase.NewConnectionString();
        var repository = TestBase.NewStudentsRepository();
        TestBase.ToDoBefore(connectionString);

        using var connection = TestBase.NewConnection(connectionString);
        connection.Open();

        FillStudentList(TestBase, connection);

        var selectQuery = repository.Select();

        //Adds a fix to pgsql test
        connection.Close();
        connection.Open();
        var resultRecords = selectQuery.Execute(connection, 0, 1).ToList();

        resultRecords.Should().ContainSingle($"The record count should be {1}, but got {resultRecords.Count}");

        connection.Close();
    }

    [Fact(DisplayName = "Test select records with not like clause")]
    public void TestSelectStudentsNotLike()
    {
        var connectionString = TestBase.NewConnectionString();
        var repository = TestBase.NewStudentsRepository();
        TestBase.ToDoBefore(connectionString);

        using var connection = TestBase.NewConnection(connectionString);
        connection.Open();

        FillStudentList(TestBase, connection);
        var countExpected = SampleData.DefaultStudentsList.Count(m => !m.Name.Contains("Doe"));
        var selectQuery = repository.Select().Where(m => m.NotLike(f => f.Name, "%Doe%"));

        //Adds a fix to pgsql test
        connection.Close();
        connection.Open();
        var resultRecords = selectQuery.Execute(connection).ToList();

        resultRecords.Should().HaveCount(countExpected, $"The record count should be {countExpected}, but got {resultRecords.Count}");

        connection.Close();
    }

    [Fact(DisplayName = "Test select records with or conditions")]
    public void TestSelectStudentsWithOrConditions()
    {
        var connectionString = TestBase.NewConnectionString();
        var repository = TestBase.NewStudentsRepository();
        TestBase.ToDoBefore(connectionString);

        using var connection = TestBase.NewConnection(connectionString);
        connection.Open();

        FillStudentList(TestBase, connection);
        var countExpected = SampleData.DefaultStudentsList.Count(m => m.Name.Contains("Doe") || m.Name.Contains("Dark"));
        var selectQuery = repository.Select().Where(m => m.Or(mg => mg.Like(f => f.Name, "%Doe%")
                                                                    .Like(f => f.Name, "%Dark%"))
                                                    );

        //Adds a fix to pgsql test
        connection.Close();
        connection.Open();
        var resultRecords = selectQuery.Execute(connection).ToList();

        resultRecords.Should().HaveCount(countExpected, $"The record count should be {countExpected}, but got {resultRecords.Count}");

        connection.Close();
    }

    [Fact(DisplayName = "Test select records with and conditions")]
    public void TestSelectStudentsWithAndConditions()
    {
        var connectionString = TestBase.NewConnectionString();
        var repository = TestBase.NewStudentsRepository();
        TestBase.ToDoBefore(connectionString);

        using var connection = TestBase.NewConnection(connectionString);
        connection.Open();

        FillStudentList(TestBase, connection);
        var countExpected = SampleData.DefaultStudentsList.Count(m => m.Name.Contains("Doe") && m.Name.Contains('J'));
        var selectQuery = repository.Select().Where(m => m.And(mg => mg.Like(f => f.Name, "%Doe%")
                                                                    .Like(f => f.Name, "%J%"))
                                                    );

        //Adds a fix to pgsql test
        connection.Close();
        connection.Open();
        var resultRecords = selectQuery.Execute(connection).ToList();

        resultRecords.Should().HaveCount(countExpected, $"The record count should be {countExpected}, but got {resultRecords.Count}");

        connection.Close();
    }

    [Fact(DisplayName = "Test select records with complex conditions")]
    public void TestSelectStudentsWithComplexConditions()
    {
        var connectionString = TestBase.NewConnectionString();
        var repository = TestBase.NewStudentsRepository();
        TestBase.ToDoBefore(connectionString);

        using var connection = TestBase.NewConnection(connectionString);
        connection.Open();

        FillStudentList(TestBase, connection);
        var countExpected = SampleData.DefaultStudentsList.Count(m => (m.Name.Contains("Doe") && m.Name.Contains('J')) || m.Name.Contains("Adams"));
        var selectQuery = repository.Select()
                                    .Where(m => m.Or(a => a.And(mg => mg.Like(f => f.Name, "%Doe%").Like(f => f.Name, "%J%"))
                                                                    .Like(f => f.Name, "%Adams%")));

        //Adds a fix to pgsql test
        connection.Close();
        connection.Open();
        var resultRecords = selectQuery.Execute(connection).ToList();

        resultRecords.Should().HaveCount(countExpected, $"The record count should be {countExpected}, but got {resultRecords.Count}");

        connection.Close();
    }

    [Fact(DisplayName = "Test select records with like clause")]
    public void TestSelectStudentsLike()
    {
        var connectionString = TestBase.NewConnectionString();
        var repository = TestBase.NewStudentsRepository();
        TestBase.ToDoBefore(connectionString);

        using var connection = TestBase.NewConnection(connectionString);
        connection.Open();

        FillStudentList(TestBase, connection);
        var countExpected = SampleData.DefaultStudentsList.Count(m => m.Name.Contains("Doe"));
        var selectQuery = repository.Select().Where(m => m.Like(f => f.Name, "%Doe%"));

        //Adds a fix to pgsql test
        connection.Close();
        connection.Open();
        var resultRecords = selectQuery.Execute(connection).ToList();

        resultRecords.Should().HaveCount(countExpected, $"The record count should be {countExpected}, but got {resultRecords.Count}");

        connection.Close();
    }

    [Fact(DisplayName = "Test select records with not between clause")]
    public void TestSelectStudentsNotBetween()
    {
        var connectionString = TestBase.NewConnectionString();
        var repository = TestBase.NewStudentsRepository();
        TestBase.ToDoBefore(connectionString);

        using var connection = TestBase.NewConnection(connectionString);
        connection.Open();

        FillStudentList(TestBase, connection);
        var countExpected = SampleData.DefaultStudentsList.Count - 5;
        var selectQuery = repository.Select().Where(m => m.NotBetween(f => f.Id, 1, 5));

        //Adds a fix to pgsql test
        connection.Close();
        connection.Open();
        var resultRecords = selectQuery.Execute(connection).ToList();

        resultRecords.Should().HaveCount(countExpected, $"The record count should be {countExpected}, but got {resultRecords.Count}");

        connection.Close();
    }

    [Fact(DisplayName = "Test select records with between clause")]
    public void TestSelectStudentsBetween()
    {
        var connectionString = TestBase.NewConnectionString();
        var repository = TestBase.NewStudentsRepository();
        TestBase.ToDoBefore(connectionString);

        using var connection = TestBase.NewConnection(connectionString);
        connection.Open();

        FillStudentList(TestBase, connection);
        var countExpected = 5;
        var selectQuery = repository.Select().Where(m => m.Between(f => f.Id, 1, 5));

        //Adds a fix to pgsql test
        connection.Close();
        connection.Open();
        var resultRecords = selectQuery.Execute(connection).ToList();

        resultRecords.Should().HaveCount(countExpected, $"The record count should be {countExpected}");

        connection.Close();
    }
}