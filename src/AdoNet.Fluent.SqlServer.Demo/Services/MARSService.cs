using System.Data;
using System.Data.Common;
using AdoNet.Fluent.SqlServer.Demo.Models;

namespace AdoNet.Fluent.SqlServer.Demo.Services;

internal sealed class MARSService(ISqlServerStatementBuilder builder) : IMARSService<Department>
{
    private readonly ISqlServerStatementBuilder _builder = builder;

    private SqlServerStatement? _statement;

    private readonly List<Department> _departments = [];

    private readonly List<Shift> _shifts = [];

    private int ordDeptId, ordName, ordGroupName, ordModifiedDate, ordShiftId, ordShiftName;

    private const string SelectDepartment = @"
        SELECT 
            DepartmentID,
            [Name],
            GroupName,
            ModifiedDate
        FROM HumanResources.Department 
        WHERE GroupName = @GroupName
    ";

    private const string SelectShift = @"
        SELECT DISTINCT
            S.ShiftID,
            S.[Name]
        FROM HumanResources.[Shift] S 
        INNER JOIN HumanResources.EmployeeDepartmentHistory EDH
        ON S.ShiftID = EDH.DepartmentID
        WHERE EDH.DepartmentID = @DepartmentID
    ";

    private void FillDepartment(DbDataReader reader)
    {
        short departmentId = reader.GetInt16(ordDeptId);
        _shifts.Clear();

        _statement?.SetParameter("DepartmentID", departmentId);
        _statement?.Read(SetOrdinalShift, FillShift);

        _departments.Add(new Department()
        {
            Id = departmentId,
            Name = reader.GetString(ordName),
            GroupName = reader.GetString(ordGroupName),
            ModifiedDate = reader.GetDateTime(ordModifiedDate),
            Shifts = new List<Shift>(_shifts)
        });
    }

    private void FillShift(DbDataReader reader)
    {
        _shifts.Add(new Shift { Id = reader.GetByte(ordShiftId), Name = reader.GetString(ordShiftName) });   
    }

    private void SetOrdinalDepartment(IDataRecord reader)
    {
        ordDeptId = reader.GetOrdinal("DepartmentID");
        ordName = reader.GetOrdinal("Name");
        ordGroupName = reader.GetOrdinal("GroupName");
        ordModifiedDate = reader.GetOrdinal("ModifiedDate");

        _statement?
            .SetSql(SelectShift)
            .AddInParameter("DepartmentID", NumericType.Int16);
    }

    private void SetOrdinalShift(IDataRecord reader)
    {
        ordShiftId = reader.GetOrdinal("ShiftID");
        ordShiftName = reader.GetOrdinal("Name");
    }

    #region IMARSService<Shift> members

    public List<Department> Read(string value)
    {
        using (_statement = _builder.WithMARS().Build())
        {
            _statement
                .SetSql(SelectDepartment)
                .AddInParameter("GroupName", value, 50)
                .Read(SetOrdinalDepartment, FillDepartment);
        }

        return _departments;
    }

    public async Task<List<Department>> ReadAsync(string value)
    {
        using (_statement = _builder.WithMARS().Build())
        {
            await _statement
                .SetSql(SelectDepartment)
                .AddInParameter("GroupName", value, 50)
                .ReadAsync(SetOrdinalDepartment, FillDepartment);
        }

        return _departments;
    }

    #endregion
}
