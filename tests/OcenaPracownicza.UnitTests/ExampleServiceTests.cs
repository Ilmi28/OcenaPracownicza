using Xunit;
using Moq;
using OcenaPracownicza.API.Services;
using OcenaPracownicza.API.Interfaces.Repositories;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Exceptions.BaseExceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace OcenaPracownicza.UnitTests
{
    public class ExampleServiceTests
    {
        private readonly Mock<IExampleRepository> _repoMock;
        private readonly ExampleService _service;

        public ExampleServiceTests()
        {
            _repoMock = new Mock<IExampleRepository>();
            _service = new ExampleService(_repoMock.Object);
        }

        [Fact]
        public async Task ExampleService_GetById_ReturnsEntity_WhenExists()
        {
            var entity = new ExampleEntity { Name = "Test", Description = "Desc", SomeDetail = "Detail" };
            _repoMock.Setup(r => r.GetById(1)).ReturnsAsync(entity);

            var result = await _service.GetById(1);

            Assert.NotNull(result);
            Assert.Equal("Test", result.Data.Name);
        }

        [Fact]
        public async Task ExampleService_GetById_ThrowsNotFoundException_WhenNotExists()
        {
            _repoMock.Setup(r => r.GetById(1)).ReturnsAsync((ExampleEntity)null!);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetById(1));
        }

        [Fact]
        public async Task ExampleService_Add_CreatesEntity()
        {
            var request = new ExampleRequest { Name = "New", Description = "Desc", SomeDetail = "Detail" };
            var entity = new ExampleEntity { Name = "New", Description = "Desc", SomeDetail = "Detail" };

            _repoMock.Setup(r => r.Create(It.IsAny<ExampleEntity>())).ReturnsAsync(entity);

            var result = await _service.Add(request);

            Assert.NotNull(result);
            Assert.Equal("New", result.Data.Name);
        }

        [Fact]
        public async Task ExampleService_Add_ThrowsArgumentNullException_WhenRequestIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.Add(null!));
        }

        [Fact]
        public async Task ExampleService_Update_UpdatesEntity_WhenExists()
        {
            var existing = new ExampleEntity { Name = "Old", Description = "OldDesc", SomeDetail = "OldDetail" };
            var request = new ExampleRequest { Name = "New", Description = "NewDesc", SomeDetail = "NewDetail" };

            _repoMock.Setup(r => r.GetById(1)).ReturnsAsync(existing);
            _repoMock.Setup(r => r.Update(existing)).ReturnsAsync(existing);

            var result = await _service.Update(1, request);

            Assert.Equal("New", result.Data.Name);
            Assert.Equal("NewDesc", result.Data.Description);
        }

        [Fact]
        public async Task ExampleService_Update_ThrowsNotFoundException_WhenNotExists()
        {
            _repoMock.Setup(r => r.GetById(1)).ReturnsAsync((ExampleEntity)null!);
            var request = new ExampleRequest { Name = "X", Description = "X", SomeDetail = "X" };

            await Assert.ThrowsAsync<NotFoundException>(() => _service.Update(1, request));
        }

        [Fact]
        public async Task ExampleService_Delete_RemovesEntity_WhenExists()
        {
            _repoMock.Setup(r => r.Exists(1)).ReturnsAsync(true);
            _repoMock.Setup(r => r.Delete(1)).Returns(Task.CompletedTask);

            var result = await _service.Delete(1);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task ExampleService_Delete_ThrowsNotFoundException_WhenNotExists()
        {
            _repoMock.Setup(r => r.Exists(1)).ReturnsAsync(false);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.Delete(1));
        }

        [Fact]
        public async Task ExampleService_Add_Throws_WhenRequestIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.Add(null!));
        }

        [Fact]
        public async Task ExampleService_GetById_Throws_WhenNotFound()
        {
            _repoMock.Setup(r => r.GetById(1)).ReturnsAsync((ExampleEntity?)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetById(1));
        }

        [Fact]
        public async Task ExampleService_Update_Throws_WhenNotFound()
        {
            _repoMock.Setup(r => r.GetById(1)).ReturnsAsync((ExampleEntity?)null);
            var request = new ExampleRequest { Name = "X", Description = "Y", SomeDetail = "Z" };
            await Assert.ThrowsAsync<NotFoundException>(() => _service.Update(1, request));
        }

        [Fact]
        public async Task ExampleService_Delete_Throws_WhenNotFound()
        {
            _repoMock.Setup(r => r.Exists(1)).ReturnsAsync(false);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.Delete(1));
        }

        [Fact]
        public async Task ExampleService_GetAll_ReturnsEmptyList_WhenNoEntities()
        {
            _repoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ExampleEntity>());
            var result = await _service.GetAll();
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task ExampleService_GetAll_ReturnsAllEntities()
        {
            var entities = new List<ExampleEntity>
            {
                new() { Name = "A", Description = "B", SomeDetail = "C" },
                new() { Name = "X", Description = "Y", SomeDetail = "Z" }
            };
            _repoMock.Setup(r => r.GetAll()).ReturnsAsync(entities);

            var result = await _service.GetAll();

            Assert.Equal(2, result.Data.Count);
            Assert.Contains(result.Data, d => d.Name == "A");
            Assert.Contains(result.Data, d => d.Name == "X");
        }

        [Fact]
        public async Task ExampleService_Add_MapsFieldsCorrectly()
        {
            var request = new ExampleRequest { Name = "Name", Description = "Desc", SomeDetail = "Detail" };
            _repoMock.Setup(r => r.Create(It.IsAny<ExampleEntity>())).ReturnsAsync((ExampleEntity e) => e);

            var result = await _service.Add(request);

            Assert.Equal("Name", result.Data.Name);
            Assert.Equal("Desc", result.Data.Description);
            Assert.Equal("Detail", result.Data.SomeDetail);
        }

        [Fact]
        public async Task ExampleService_Update_MapsFieldsCorrectly()
        {
            var existing = new ExampleEntity { Name = "Old", Description = "OldDesc", SomeDetail = "OldDetail" };
            _repoMock.Setup(r => r.GetById(1)).ReturnsAsync(existing);
            _repoMock.Setup(r => r.Update(It.IsAny<ExampleEntity>())).ReturnsAsync((ExampleEntity e) => e);

            var request = new ExampleRequest { Name = "New", Description = "NewDesc", SomeDetail = "NewDetail" };
            var result = await _service.Update(1, request);

            Assert.Equal("New", result.Data.Name);
            Assert.Equal("NewDesc", result.Data.Description);
            Assert.Equal("NewDetail", result.Data.SomeDetail);
        }

        [Fact]
        public async Task ExampleService_Add_Throws_WhenRepositoryThrows()
        {
            var request = new ExampleRequest { Name = "X", Description = "Y", SomeDetail = "Z" };
            _repoMock.Setup(r => r.Create(It.IsAny<ExampleEntity>())).ThrowsAsync(new InvalidOperationException());

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Add(request));
        }
    }
}
