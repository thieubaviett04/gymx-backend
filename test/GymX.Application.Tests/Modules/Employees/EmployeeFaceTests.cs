using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Employees.Commands.RegisterEmployeeFace;
using GymX.Application.Modules.Employees.Commands.RemoveEmployeeFace;
using GymX.Application.Modules.Employees.DTOs.Response;
using GymX.Application.Modules.Users.DTOs.Response;
using GymX.Application.Modules.Employees.Queries.GetEmployeeFaceDetail;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants.Identity;
using GymX.Domain.Entities.Employees;
using GymX.Domain.Entities.Identity;
using GymX.Domain.Errors;

namespace GymX.Application.Tests.Modules.Employees;

public class EmployeeFaceTests
{
    [Fact]
    public void Validator_ShouldFail_WhenEmployeeIdIsEmpty()
    {
        var validator = new RegisterEmployeeFaceCommandValidator();
        var command = new RegisterEmployeeFaceCommand(
            Guid.Empty,
            [1, 2, 3],
            "face.jpg",
            "image/jpeg",
            3);

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.EmployeeId));
    }

    [Fact]
    public void Validator_ShouldFail_WhenFileSizeIsZero()
    {
        var validator = new RegisterEmployeeFaceCommandValidator();
        var command = new RegisterEmployeeFaceCommand(
            Guid.NewGuid(),
            [],
            "face.jpg",
            "image/jpeg",
            0);

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.FileSize));
    }

    [Fact]
    public void Validator_ShouldFail_WhenFileSizeExceeds5MB()
    {
        var validator = new RegisterEmployeeFaceCommandValidator();
        var command = new RegisterEmployeeFaceCommand(
            Guid.NewGuid(),
            [1, 2, 3],
            "face.jpg",
            "image/jpeg",
            6 * 1024 * 1024);

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.FileSize));
    }

    [Fact]
    public void Validator_ShouldFail_WhenInvalidExtension()
    {
        var validator = new RegisterEmployeeFaceCommandValidator();
        var command = new RegisterEmployeeFaceCommand(
            Guid.NewGuid(),
            [1, 2, 3],
            "document.pdf",
            "image/jpeg",
            100);

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.FileName));
    }

    [Fact]
    public void Validator_ShouldFail_WhenInvalidMimeType()
    {
        var validator = new RegisterEmployeeFaceCommandValidator();
        var command = new RegisterEmployeeFaceCommand(
            Guid.NewGuid(),
            [1, 2, 3],
            "face.jpg",
            "application/pdf",
            100);

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.ContentType));
    }

    [Fact]
    public void Validator_ShouldSucceed_WhenValidInput()
    {
        var validator = new RegisterEmployeeFaceCommandValidator();
        var command = new RegisterEmployeeFaceCommand(
            Guid.NewGuid(),
            [1, 2, 3],
            "face.png",
            "image/png",
            1024);

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task RegisterFace_ShouldReturnNotFound_WhenEmployeeDoesNotExist()
    {
        var empRepo = new FakeEmployeeRepository();
        var userRepo = new FakeUserRepository();
        var faceRepo = new FakeFaceDataRepository();
        var faceService = new FakeFaceRecognitionService();
        var currentUserService = new FakeCurrentUserService();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new RegisterEmployeeFaceCommandHandler(
            empRepo,
            userRepo,
            faceRepo,
            faceService,
            currentUserService,
            unitOfWork);

        var command = new RegisterEmployeeFaceCommand(
            Guid.NewGuid(),
            [1, 2, 3],
            "face.jpg",
            "image/jpeg",
            3);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(EmployeeFaceErrors.EmployeeNotFound.Code, result.Error.Code);
    }

    [Fact]
    public async Task RegisterFace_ShouldReturnNotFound_WhenEmployeeIsDeleted()
    {
        var empRepo = new FakeEmployeeRepository();
        var userRepo = new FakeUserRepository();
        var faceRepo = new FakeFaceDataRepository();
        var faceService = new FakeFaceRecognitionService();
        var currentUserService = new FakeCurrentUserService();
        var unitOfWork = new FakeUnitOfWork();

        var employeeId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var employee = Employee.Create(userId, "NV0001", "STAFF", 5000000, new DateOnly(2025, 1, 1));
        employee.SoftDelete();
        empRepo.Employees[employeeId] = employee;

        var handler = new RegisterEmployeeFaceCommandHandler(
            empRepo,
            userRepo,
            faceRepo,
            faceService,
            currentUserService,
            unitOfWork);

        var command = new RegisterEmployeeFaceCommand(
            employeeId,
            [1, 2, 3],
            "face.jpg",
            "image/jpeg",
            3);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(EmployeeFaceErrors.EmployeeNotFound.Code, result.Error.Code);
    }

    [Fact]
    public async Task RegisterFace_ShouldReturnError_WhenFaceRecognitionDetectsNoFace()
    {
        var empRepo = new FakeEmployeeRepository();
        var userRepo = new FakeUserRepository();
        var faceRepo = new FakeFaceDataRepository();
        var faceService = new FakeFaceRecognitionService
        {
            RegisterResult = Result<string>.Failure(EmployeeFaceErrors.NoFaceFound)
        };
        var currentUserService = new FakeCurrentUserService();
        var unitOfWork = new FakeUnitOfWork();

        var employeeId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var employee = Employee.Create(userId, "NV0001", "STAFF", 5000000, new DateOnly(2025, 1, 1));
        empRepo.Employees[employeeId] = employee;

        var user = User.CreateWithPassword("test@gymx.com", "Nguyen Van A", "hash");
        userRepo.Users[userId] = user;

        var handler = new RegisterEmployeeFaceCommandHandler(
            empRepo,
            userRepo,
            faceRepo,
            faceService,
            currentUserService,
            unitOfWork);

        var command = new RegisterEmployeeFaceCommand(
            employeeId,
            [1, 2, 3],
            "face.jpg",
            "image/jpeg",
            3);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(EmployeeFaceErrors.NoFaceFound.Code, result.Error.Code);
    }

    [Fact]
    public async Task RegisterFace_ShouldReturnError_WhenFaceRecognitionDetectsMultipleFaces()
    {
        var empRepo = new FakeEmployeeRepository();
        var userRepo = new FakeUserRepository();
        var faceRepo = new FakeFaceDataRepository();
        var faceService = new FakeFaceRecognitionService
        {
            RegisterResult = Result<string>.Failure(EmployeeFaceErrors.MultipleFacesDetected)
        };
        var currentUserService = new FakeCurrentUserService();
        var unitOfWork = new FakeUnitOfWork();

        var employeeId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var employee = Employee.Create(userId, "NV0001", "STAFF", 5000000, new DateOnly(2025, 1, 1));
        empRepo.Employees[employeeId] = employee;

        var user = User.CreateWithPassword("test@gymx.com", "Nguyen Van A", "hash");
        userRepo.Users[userId] = user;

        var handler = new RegisterEmployeeFaceCommandHandler(
            empRepo,
            userRepo,
            faceRepo,
            faceService,
            currentUserService,
            unitOfWork);

        var command = new RegisterEmployeeFaceCommand(
            employeeId,
            [1, 2, 3],
            "face.jpg",
            "image/jpeg",
            3);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(EmployeeFaceErrors.MultipleFacesDetected.Code, result.Error.Code);
    }

    [Fact]
    public async Task RegisterFace_ShouldEnrollSuccessfully_WhenNewFace()
    {
        var empRepo = new FakeEmployeeRepository();
        var userRepo = new FakeUserRepository();
        var faceRepo = new FakeFaceDataRepository();
        var faceService = new FakeFaceRecognitionService
        {
            RegisterResult = Result<string>.Success("face_token_12345")
        };
        var currentUserService = new FakeCurrentUserService { UserId = Guid.NewGuid().ToString() };
        var unitOfWork = new FakeUnitOfWork();

        var employeeId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var employee = Employee.Create(userId, "NV0001", "STAFF", 5000000, new DateOnly(2025, 1, 1));
        empRepo.Employees[employeeId] = employee;

        var user = User.CreateWithPassword("test@gymx.com", "Nguyen Van A", "hash");
        userRepo.Users[userId] = user;

        var handler = new RegisterEmployeeFaceCommandHandler(
            empRepo,
            userRepo,
            faceRepo,
            faceService,
            currentUserService,
            unitOfWork);

        var command = new RegisterEmployeeFaceCommand(
            employeeId,
            [1, 2, 3],
            "face.jpg",
            "image/jpeg",
            3);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.HasFaceEnrolled);
        Assert.Equal("face_token_12345", result.Value.ExternalFaceId);
        Assert.Equal(FaceDataStatus.Active, result.Value.Status);
        Assert.Equal(1, unitOfWork.SaveCount);
    }

    [Fact]
    public async Task RegisterFace_ShouldUpdateExistingFace_WhenReenrolling()
    {
        var empRepo = new FakeEmployeeRepository();
        var userRepo = new FakeUserRepository();
        var faceRepo = new FakeFaceDataRepository();
        var faceService = new FakeFaceRecognitionService
        {
            RegisterResult = Result<string>.Success("new_face_token_999")
        };
        var currentUserService = new FakeCurrentUserService { UserId = Guid.NewGuid().ToString() };
        var unitOfWork = new FakeUnitOfWork();

        var employeeId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var employee = Employee.Create(userId, "NV0001", "STAFF", 5000000, new DateOnly(2025, 1, 1));
        empRepo.Employees[employeeId] = employee;

        var user = User.CreateWithPassword("test@gymx.com", "Nguyen Van A", "hash");
        userRepo.Users[userId] = user;

        var existingFace = FaceData.Create(userId, "old_face_token_111", "old_face_token_111");
        existingFace.Deactivate();
        faceRepo.FaceDataByUserId[userId] = existingFace;

        var handler = new RegisterEmployeeFaceCommandHandler(
            empRepo,
            userRepo,
            faceRepo,
            faceService,
            currentUserService,
            unitOfWork);

        var command = new RegisterEmployeeFaceCommand(
            employeeId,
            [1, 2, 3],
            "face.jpg",
            "image/jpeg",
            3);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.HasFaceEnrolled);
        Assert.Equal("new_face_token_999", result.Value.ExternalFaceId);
        Assert.Equal(FaceDataStatus.Active, result.Value.Status);
        Assert.Equal(FaceDataStatus.Active, existingFace.Status);
        Assert.Equal("new_face_token_999", existingFace.ExternalFaceId);
        Assert.Equal(1, unitOfWork.SaveCount);
    }

    [Fact]
    public async Task GetFaceDetail_ShouldReturnNotEnrolled_WhenNoFaceData()
    {
        var empRepo = new FakeEmployeeRepository();
        var userRepo = new FakeUserRepository();
        var faceRepo = new FakeFaceDataRepository();

        var employeeId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var employee = Employee.Create(userId, "NV0001", "STAFF", 5000000, new DateOnly(2025, 1, 1));
        empRepo.Employees[employeeId] = employee;

        var user = User.CreateWithPassword("test@gymx.com", "Nguyen Van A", "hash");
        userRepo.Users[userId] = user;

        var handler = new GetEmployeeFaceDetailQueryHandler(empRepo, userRepo, faceRepo);
        var query = new GetEmployeeFaceDetailQuery(employeeId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value.HasFaceEnrolled);
        Assert.Equal("NOT_ENROLLED", result.Value.Status);
        Assert.Null(result.Value.ExternalFaceId);
    }

    [Fact]
    public async Task GetFaceDetail_ShouldReturnActive_WhenEnrolled()
    {
        var empRepo = new FakeEmployeeRepository();
        var userRepo = new FakeUserRepository();
        var faceRepo = new FakeFaceDataRepository();

        var employeeId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var employee = Employee.Create(userId, "NV0001", "STAFF", 5000000, new DateOnly(2025, 1, 1));
        empRepo.Employees[employeeId] = employee;

        var user = User.CreateWithPassword("test@gymx.com", "Nguyen Van A", "hash");
        userRepo.Users[userId] = user;

        var face = FaceData.Create(userId, "face_token_active", "face_token_active");
        faceRepo.FaceDataByUserId[userId] = face;

        var handler = new GetEmployeeFaceDetailQueryHandler(empRepo, userRepo, faceRepo);
        var query = new GetEmployeeFaceDetailQuery(employeeId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.HasFaceEnrolled);
        Assert.Equal(FaceDataStatus.Active, result.Value.Status);
        Assert.Equal("face_token_active", result.Value.ExternalFaceId);
    }

    [Fact]
    public async Task RemoveFace_ShouldReturnNotFound_WhenNoFaceData()
    {
        var empRepo = new FakeEmployeeRepository();
        var faceRepo = new FakeFaceDataRepository();
        var unitOfWork = new FakeUnitOfWork();

        var employeeId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var employee = Employee.Create(userId, "NV0001", "STAFF", 5000000, new DateOnly(2025, 1, 1));
        empRepo.Employees[employeeId] = employee;

        var handler = new RemoveEmployeeFaceCommandHandler(empRepo, faceRepo, unitOfWork);
        var command = new RemoveEmployeeFaceCommand(employeeId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(EmployeeFaceErrors.FaceDataNotFound.Code, result.Error.Code);
    }

    [Fact]
    public async Task RemoveFace_ShouldDeactivateFace_WhenActive()
    {
        var empRepo = new FakeEmployeeRepository();
        var faceRepo = new FakeFaceDataRepository();
        var unitOfWork = new FakeUnitOfWork();

        var employeeId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var employee = Employee.Create(userId, "NV0001", "STAFF", 5000000, new DateOnly(2025, 1, 1));
        empRepo.Employees[employeeId] = employee;

        var face = FaceData.Create(userId, "face_token_active", "face_token_active");
        faceRepo.FaceDataByUserId[userId] = face;

        var handler = new RemoveEmployeeFaceCommandHandler(empRepo, faceRepo, unitOfWork);
        var command = new RemoveEmployeeFaceCommand(employeeId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(FaceDataStatus.Inactive, face.Status);
        Assert.Equal(1, unitOfWork.SaveCount);
    }
}

public class FakeEmployeeRepository : IEmployeeRepository
{
    public Dictionary<Guid, Employee> Employees { get; } = new();

    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Employees.TryGetValue(id, out var emp);
        return Task.FromResult(emp);
    }

    public Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Employee>>(Employees.Values.ToList());
    }

    public Task AddAsync(Employee entity, CancellationToken cancellationToken = default)
    {
        Employees[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task AddRangeAsync(IEnumerable<Employee> entities, CancellationToken cancellationToken = default)
    {
        foreach (var e in entities) Employees[e.Id] = e;
        return Task.CompletedTask;
    }

    public void Update(Employee entity) { }
    public void Delete(Employee entity) { }

    public Task<Employee?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Employees.Values.FirstOrDefault(e => e.UserId == userId));
    }

    public Task<Employee?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Employees.Values.FirstOrDefault(e => e.NationalId == nationalId));
    }

    public Task<Employee?> GetByEmployeeCodeAsync(string employeeCode, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Employees.Values.FirstOrDefault(e => e.EmployeeCode == employeeCode));
    }

    public Task<string> GenerateNextEmployeeCodeAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult("NV0001");
    }

    public Task<PagedResult<EmployeeSummaryDto>> GetEmployeesPagedAsync(string? search, string? position, string? status, int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new PagedResult<EmployeeSummaryDto>(new List<EmployeeSummaryDto>(), 0, pageIndex, pageSize));
    }

    public Task<EmployeeDetailDto?> GetEmployeeDetailByIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<EmployeeDetailDto?>(null);
    }

    public Task<EmployeeProfileDto?> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<EmployeeProfileDto?>(null);
    }

    public Task<List<Employee>> GetActiveByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<Employee>());
    }
}

public class FakeUserRepository : IUserRepository
{
    public Dictionary<Guid, User> Users { get; } = new();

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<User>>(Users.Values.ToList());
    }

    public Task AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        Users[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task AddRangeAsync(IEnumerable<User> entities, CancellationToken cancellationToken = default)
    {
        foreach (var u in entities) Users[u.Id] = u;
        return Task.CompletedTask;
    }

    public void Update(User entity) { }
    public void Delete(User entity) { }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Users.Values.FirstOrDefault(u => u.Email == email));
    }

    public Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Users.Values.FirstOrDefault(u => u.GoogleId == googleId));
    }

    public Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Users.Values.FirstOrDefault(u => u.PhoneNumber == phoneNumber));
    }

    public Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<string>());
    }

    public Task<UserProfileDto?> GetFullProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<UserProfileDto?>(null);
    }

    public Task<User?> GetUserWithRolesByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        Users.TryGetValue(userId, out var user);
        return Task.FromResult(user);
    }
}

public class FakeFaceDataRepository : IFaceDataRepository
{
    public Dictionary<Guid, FaceData> FaceDataByUserId { get; } = new();

    public Task<FaceData?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        FaceDataByUserId.TryGetValue(userId, out var face);
        return Task.FromResult(face);
    }

    public Task<FaceData?> GetByExternalFaceIdAsync(string externalFaceId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(FaceDataByUserId.Values.FirstOrDefault(f => f.ExternalFaceId == externalFaceId));
    }

    public Task<FaceData?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<FaceData?>(null);
    }

    public Task<IReadOnlyList<FaceData>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<FaceData>>(FaceDataByUserId.Values.ToList());
    }

    public Task AddAsync(FaceData entity, CancellationToken cancellationToken = default)
    {
        FaceDataByUserId[entity.UserId] = entity;
        return Task.CompletedTask;
    }

    public Task AddRangeAsync(IEnumerable<FaceData> entities, CancellationToken cancellationToken = default)
    {
        foreach (var f in entities) FaceDataByUserId[f.UserId] = f;
        return Task.CompletedTask;
    }

    public void Update(FaceData entity)
    {
        FaceDataByUserId[entity.UserId] = entity;
    }

    public void Delete(FaceData entity)
    {
        FaceDataByUserId.Remove(entity.UserId);
    }
}

public class FakeFaceRecognitionService : IFaceRecognitionService
{
    public Result<string> RegisterResult { get; set; } = Result<string>.Success("test_face_token");
    public Result<string> IdentifyResult { get; set; } = Result<string>.Success("test_face_token");
    public Result<double> CompareResult { get; set; } = Result<double>.Success(0.0);

    public Task<Result<string>> RegisterFaceAsync(byte[] imageBytes, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(RegisterResult);
    }

    public Task<Result<string>> IdentifyFaceAsync(
        byte[] imageBytes,
        IEnumerable<string>? candidateFaceTokens = null,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(IdentifyResult);
    }

    public Task<Result<double>> CompareFacesAsync(
        string faceToken1,
        string faceToken2,
        CancellationToken cancellationToken = default)
    {
        if (faceToken1 == faceToken2)
        {
            return Task.FromResult(Result<double>.Success(100.0));
        }
        return Task.FromResult(CompareResult);
    }
}

public class FakeCurrentUserService : ICurrentUserService
{
    public string? UserId { get; set; }
    public string? Email { get; set; }
}

public class FakeUnitOfWork : IUnitOfWork
{
    public int SaveCount { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveCount++;
        return Task.FromResult(1);
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task CommitTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default) => await action();
}
