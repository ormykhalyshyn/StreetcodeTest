﻿using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.MediatR.AdditionalContent.GetById;
using Streetcode.BLL.MediatR.AdditionalContent.Subtitle.GetById;
using Streetcode.DAL.Entities.AdditionalContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.SubtitleTests
{
    public class GetSubtitleByIdRequestHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;

        public GetSubtitleByIdRequestHandlerTests()
        {
            _mockRepo = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
        }

        private const int _id = 1;

        private readonly Subtitle subtitle = new Subtitle { Id = _id };
        private readonly SubtitleDTO subtitleDTO = new SubtitleDTO { Id = _id };
        async Task SetupRepository(Subtitle returnElement)
        {
            _mockRepo.Setup(repo => repo.SubtitleRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Subtitle, bool>>>(),
                It.IsAny<Func<IQueryable<Subtitle>,
                IIncludableQueryable<Subtitle, object>>>()))
                .ReturnsAsync(returnElement);
        }
        async Task SetupMapper(SubtitleDTO returnElement)
        {
            _mockMapper.Setup(x => x.Map<SubtitleDTO>(It.IsAny<object>()))
                .Returns(returnElement);
        }

        [Fact]
        public async Task Handler_Returns_Matching_Element()
        {
            //Arrange
            await SetupRepository(subtitle);
            await SetupMapper(subtitleDTO);

            var handler = new GetSubtitleByIdHandler(_mockRepo.Object, _mockMapper.Object);
            
            //Act
            var result = await handler.Handle(new GetSubtitleByIdQuery(_id), CancellationToken.None);

            //Assert
            Assert.Multiple(
                () => Assert.IsType<SubtitleDTO>(result.Value),
                () => Assert.True(result.Value.Id.Equals(_id)));
        }

        [Fact]
        public async Task Handler_Returns_NoMatching_Element()
        {
            //Arrange
            await SetupRepository(new Subtitle());
            await SetupMapper(new SubtitleDTO());

            var handler = new GetSubtitleByIdHandler(_mockRepo.Object, _mockMapper.Object);

            //Act
            var result = await handler.Handle(new GetSubtitleByIdQuery(_id), CancellationToken.None);

            //Assert
            Assert.Multiple(
                () => Assert.NotNull(result),
                () => Assert.False(result.Value.Id.Equals(_id)));
        }
    }
}
