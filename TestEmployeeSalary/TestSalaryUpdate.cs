using System.Xml.Linq;

namespace TestEmployeeSalary
{

    [TestFixture]
    public class TestSalaryUpdate
    {
        //Set database access parameters
        private readonly string dbName = "employees_test123";
        private readonly string server = "localhost";
        private readonly string uid = "root";
        private readonly string password = "107EquityScam";

        //Employee details
        private readonly string customerName = "David";
        private readonly int originalSalary = 1000;
        private readonly int newSalary = 2000;
        private int empId;

        private DbSetup dbSetup;
        private EmployeeDAL employeeDAL;

        [SetUp]
        public void Setup()
        {
            string CS = $"server={server};uid={uid};password={password};";
            dbSetup = new DbSetup(CS);

            // Create test database, table, and procedure
            dbSetup.Initialize(dbName);

            //Reset connection string to add database just created
            CS += $"database={dbName};";

            // Initialize Employee data access layer
            employeeDAL = new EmployeeDAL(CS);
            empId = employeeDAL.CreateEmployee(customerName, originalSalary);
        }

        [Test]
        public void TestEmployeeWasCreatedAndAssignedCorrectSalary()
        {
            int salaryFromDb = employeeDAL.GetEmployeeSalary(empId);
            Assert.That(salaryFromDb, Is.EqualTo(originalSalary));
        }

        [Test]
        public void TestUpdateEmployeeSalaryWithInvalidIdShouldThrowException()
        {
            int invalidId = 0;
            var errorMessage = $"Employee with id {invalidId} does not exist";
            var ex = Assert.Throws<InvalidDataException>(() => employeeDAL.UpdateEmployeeSalary(invalidId, newSalary));
            Assert.That(ex.Message, Is.EqualTo(errorMessage));
        }


        [Test]
        public void TestUpdateEmployeeSalaryWithValidDetailsShouldReturnOldSalary()
        {
            var salaryBeforeUpdate = employeeDAL.GetEmployeeSalary(empId);
            var oldSalary = employeeDAL.UpdateEmployeeSalary(empId, newSalary);
            Assert.That(oldSalary, Is.EqualTo(salaryBeforeUpdate));
         
        }

        [Test]
        public void TestUpdateEmployeeSalaryWithValidDetailsCommitsNewSalaryToDb()
        {
            employeeDAL.UpdateEmployeeSalary(empId, newSalary);
            var updatedSalary = employeeDAL.GetEmployeeSalary(empId);
            Assert.That(updatedSalary, Is.EqualTo(newSalary));
        }



        [TearDown]
        public void TearDown()
        {
            // Drop test database
            dbSetup.DropDatabase(dbName);
        }
    }
}