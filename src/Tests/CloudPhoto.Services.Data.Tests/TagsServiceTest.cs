namespace CloudPhoto.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CloudPhoto.Data;
    using CloudPhoto.Data.Common.Repositories;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Data.Repositories;
    using TagsService;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class TagsServiceTest : IDisposable
    {
        private const string TestTagName1 = "Tag1";
        private const string TestTagName2 = "Tag2";
        private const string TestTagPartialName = "Tag";

        private string testTagId1;
        private string testTagId2;

        private IDeletableEntityRepository<Tag> tagRepository;
        private TagsService tagService;

        public TagsServiceTest()
        {
            InitTestServices();

            AddTestData();
        }

        [Fact]
        public async void CreateTag()
        {
            string newTadId = await tagService.CreateAsync("Nature Tag Name", "description", "userId");
            Tag selectNewTag = tagService.GetByTagName<Tag>("Nature Tag Name");
            Assert.Equal(newTadId, selectNewTag?.Id);
        }

        [Fact]
        public async void CreateTagOnlyWithName()
        {
            string newTadId = await tagService.CreateAsync("Name", null, null);
            Assert.NotNull(newTadId);
        }

        [Fact]
        public void GetTagByFullName()
        {
            Tag selectTag = tagService.GetByTagName<Tag>(TestTagName1);
            Assert.Equal(testTagId1, selectTag?.Id);
        }

        [Fact]
        public void GetTagByUndefineName()
        {
            Tag selectTag = tagService.GetByTagName<Tag>("undefine");
            Assert.Null(selectTag);
        }

        [Fact]
        public void FilteTagByFullName()
        {
            IEnumerable<Tag> selectTags = tagService.FiterTagsByNames<Tag>(TestTagName1);
            Assert.Single(selectTags);
        }

        [Fact]
        public void FilteTagByName()
        {
            IEnumerable<Tag> selectTags = tagService.FiterTagsByNames<Tag>(TestTagName2);
            Assert.Equal(selectTags.ToList()[0].Id, testTagId2);
        }

        [Fact]
        public void FilteTagByPartialName()
        {
            IEnumerable<Tag> selectTags = tagService.FiterTagsByNames<Tag>(TestTagPartialName);
            Assert.Equal(2, selectTags?.Count());
        }

        [Fact]
        public void FilteTagByUndefineName()
        {
            IEnumerable<Tag> selectTags = tagService.FiterTagsByNames<Tag>("undefine");
            Assert.Empty(selectTags);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                tagRepository.Dispose();
            }
        }

        private void InitTestServices()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString());

            ApplicationDbContext dbContext = new ApplicationDbContext(options.Options);

            tagRepository = new EfDeletableEntityRepository<Tag>(dbContext);

            tagService = new TagsService(
                tagRepository);
        }

        private async void AddTestData()
        {
            testTagId1 = await tagService.CreateAsync(TestTagName1, null, "userId1");
            testTagId2 = await tagService.CreateAsync(TestTagName2, null, "userId2");
        }
    }
}
