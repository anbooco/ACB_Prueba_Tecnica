using ACB_Prueba_Tecnica.Domain.Entities;
using ACB_Prueba_Tecnica.Domain.Interfaces.Repositories;
using ACB_Prueba_Tecnica.Services;
using ACB_Prueba_Tecnica.Tests.Mocks;
using Moq;
using Xunit;

namespace ACB_Prueba_Tecnica.Tests.Services
{
    public class ACBServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<IPlayByPlayLeanRepository> _mockRepo;
        private readonly ACBService _service;

        public ACBServiceTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockRepo = new Mock<IPlayByPlayLeanRepository>();
            _service = new ACBService(_mockConfig.Object, _mockHttpClientFactory.Object, _mockRepo.Object);
        }

        #region GetPlayByPlayLean
        [Fact]
        public async Task GetPlayByPlayLean_ShouldReturnDataFromRepository_WhenPlayByPlayDataExists()
        {
            // Arrange
            var gameId = 1;
            var playByPlayData = "[{\"team\": {\"id_team_denomination\": 1}, \"id_license\": 2, \"crono\": \"00:01\", \"id_playbyplaytype\": 3}]";
            var matchEvent = new MatchEvent { PlayByPlayData = playByPlayData };

            _mockRepo.Setup(repo => repo.GetByMatchName(gameId)).ReturnsAsync(matchEvent);

            // Act
            var result = await _service.GetPlayByPlayLean(gameId);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0]["id_team_denomination"]);
            Assert.Equal(2, result[0]["id_license"]);
            Assert.Equal("00:01", result[0]["crono"]);
            Assert.Equal(3, result[0]["id_playbyplaytype"]);
        }

        [Fact]
        public async Task GetPlayByPlayLean_ShouldFetchDataFromAPI_WhenPlayByPlayDataDoesNotExist()
        {
            // Arrange
            var gameId = 1;
            var apiResponse = "[{\"team\": {\"id_team_denomination\": 1}, \"id_license\": 2, \"crono\": \"00:01\", \"id_playbyplaytype\": 3}]";

            _mockRepo.Setup(repo => repo.GetByMatchName(gameId)).ReturnsAsync((MatchEvent)null);
            _mockConfig.Setup(config => config["ACBApiUrl"]).Returns("http://fakeapi.com");
            _mockConfig.Setup(config => config["ACBApiToken"]).Returns("faketoken");
            _mockHttpClientFactory.Setup(clientFactory => clientFactory.CreateClient(It.IsAny<string>()))
                                  .Returns(new HttpClient(new MockHttpMessageHandler(apiResponse)));

            _mockRepo.Setup(repo => repo.Add(It.IsAny<MatchEvent>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.GetPlayByPlayLean(gameId);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0]["id_team_denomination"]);
            Assert.Equal(2, result[0]["id_license"]);
            Assert.Equal("00:01", result[0]["crono"]);
            Assert.Equal(3, result[0]["id_playbyplaytype"]);
        }

        [Fact]
        public async Task GetPlayByPlayLean_ShouldAvoidItem_WhenIdLicenceIsNull()
        {
            // Arrange
            var gameId = 1;
            var playByPlayData = "[{\"team\": {\"id_team_denomination\": 1}, \"id_license\": null, \"crono\": \"00:01\", \"id_playbyplaytype\": 3}]";
            var matchEvent = new MatchEvent { PlayByPlayData = playByPlayData };

            _mockRepo.Setup(repo => repo.GetByMatchName(gameId)).ReturnsAsync(matchEvent);

            // Act
            var result = await _service.GetPlayByPlayLean(gameId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetPlayByPlayLean_ShouldAvoidItem_WhenTeamIsNull()
        {
            // Arrange
            var gameId = 1;
            var playByPlayData = "[{\"team\": null, \"id_license\": 2, \"crono\": \"00:01\", \"id_playbyplaytype\": 3}]";
            var matchEvent = new MatchEvent { PlayByPlayData = playByPlayData };

            _mockRepo.Setup(repo => repo.GetByMatchName(gameId)).ReturnsAsync(matchEvent);

            // Act
            var result = await _service.GetPlayByPlayLean(gameId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetPlayByPlayLean_ShouldReturnEmptyList_WhenExceptionIsThrown()
        {
            // Arrange
            var gameId = 1;
            _mockRepo.Setup(repo => repo.GetByMatchName(gameId)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _service.GetPlayByPlayLean(gameId);

            // Assert
            Assert.Empty(result);
        }
        #endregion

        #region GetGameLeaders
        [Fact]
        public async Task GetGameLeaders_ShouldReturnDataFromRepository_WhenPlayByPlayDataExists()
        {
            // Arrange
            var gameId = 1;
            var playByPlayData = "[{\"id_license\": 1, \"statistics\": {\"points\": 25, \"total_rebound\": 10}, \"local\": true}]";
            var matchEvent = new MatchEvent { PlayByPlayData = playByPlayData };

            _mockRepo.Setup(repo => repo.GetByMatchName(gameId)).ReturnsAsync(matchEvent);

            // Act
            var result = await _service.GetGameLeaders(gameId);

            // Assert
            Assert.NotEmpty(result);
            Assert.True(result.ContainsKey("home_team_leaders"));
            Assert.True(result.ContainsKey("away_team_leaders"));
            Assert.Single((List<Dictionary<string, object>>)result["home_team_leaders"]);
            Assert.Empty((List<Dictionary<string, object>>)result["away_team_leaders"]);
        }

        [Fact]
        public async Task GetGameLeaders_ShouldFetchDataFromAPI_WhenPlayByPlayDataDoesNotExist()
        {
            // Arrange
            var gameId = 1;
            var apiResponse = "[{\"id_license\": 1, \"statistics\": {\"points\": 25, \"total_rebound\": 10}, \"local\": true}]";

            _mockRepo.Setup(repo => repo.GetByMatchName(gameId)).ReturnsAsync((MatchEvent)null);
            _mockConfig.Setup(config => config["ACBApiUrl"]).Returns("http://fakeapi.com");
            _mockConfig.Setup(config => config["ACBApiToken"]).Returns("faketoken");
            _mockHttpClientFactory.Setup(clientFactory => clientFactory.CreateClient(It.IsAny<string>()))
                                  .Returns(new HttpClient(new MockHttpMessageHandler(apiResponse)));

            // Act
            var result = await _service.GetGameLeaders(gameId);

            // Assert
            Assert.NotEmpty(result);
            Assert.True(result.ContainsKey("home_team_leaders"));
            Assert.True(result.ContainsKey("away_team_leaders"));
            Assert.Single((List<Dictionary<string, object>>)result["home_team_leaders"]);
            Assert.Empty((List<Dictionary<string, object>>)result["away_team_leaders"]);
        }

        [Fact]
        public async Task GetGameLeaders_ShouldAvoidItem_WhenStaticsIsNull()
        {
            // Arrange
            var gameId = 1;
            var playByPlayData = "[{\"id_license\": 1, \"statistics\": null, \"local\": true}]";
            var matchEvent = new MatchEvent { PlayByPlayData = playByPlayData };

            _mockRepo.Setup(repo => repo.GetByMatchName(gameId)).ReturnsAsync(matchEvent);

            // Act
            var result = await _service.GetPlayByPlayLean(gameId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetGameLeaders_ShouldAvoidItem_WhenIdLicenceIsNull()
        {
            // Arrange
            var gameId = 1;
            var playByPlayData = "[{\"id_license\": null, \"statistics\": {\"points\": 25, \"total_rebound\": 10}, \"local\": true}]";
            var matchEvent = new MatchEvent { PlayByPlayData = playByPlayData };

            _mockRepo.Setup(repo => repo.GetByMatchName(gameId)).ReturnsAsync(matchEvent);

            // Act
            var result = await _service.GetPlayByPlayLean(gameId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetGameLeaders_ShouldReturnEmptyDictionary_WhenExceptionIsThrown()
        {
            // Arrange
            var gameId = 1;
            _mockRepo.Setup(repo => repo.GetByMatchName(gameId)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _service.GetGameLeaders(gameId);

            // Assert
            Assert.Empty(result);
        }
        #endregion

        #region GetGameBiggestLeaders
        [Fact]
        public async Task GetGameBiggestLeaders_ShouldReturnCorrectLeads_WhenDataIsValid()
        {
            // Arrange
            var gameId = 1;
            var apiResponse = "[{\"score_local\": 100, \"score_visitor\": 90}, {\"score_local\": 110, \"score_visitor\": 85}]";

            _mockConfig.Setup(config => config["ACBApiUrl"]).Returns("http://fakeapi.com");
            _mockConfig.Setup(config => config["ACBApiToken"]).Returns("faketoken");
            _mockHttpClientFactory.Setup(clientFactory => clientFactory.CreateClient(It.IsAny<string>()))
                                  .Returns(new HttpClient(new MockHttpMessageHandler(apiResponse)));

            // Act
            var result = await _service.GetGameBiggestLeaders(gameId);

            // Assert
            Assert.Equal(25, result["home_team"]); // 110 - 85
            Assert.Equal(0, result["away_team"]);
        }

        [Fact]
        public async Task GetGameBiggestLeaders_ShouldReturnZeroes_WhenExceptionIsThrown()
        {
            // Arrange
            var gameId = 1;
            _mockRepo.Setup(repo => repo.GetByMatchName(gameId)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _service.GetGameBiggestLeaders(gameId);

            // Assert
            Assert.Equal(0, result["home_team"]);
            Assert.Equal(0, result["away_team"]);
        }
        #endregion
    }
}
