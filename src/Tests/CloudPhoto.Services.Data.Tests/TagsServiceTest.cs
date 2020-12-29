namespace CloudPhoto.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CloudPhoto.Data;
    using CloudPhoto.Data.Common.Repositories;
    using CloudPhoto.Data.Models;
    using CloudPhoto.Data.Repositories;
    using CloudPhoto.Services.Data.TagsService;
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
            this.InitTestServices();

            this.AddTestData();
        }

        [Fact]
        public async void CreateTag()
        {
            string newTadId = await this.tagService.CreateAsync("Nature Tag Name", "description", "userId");
            Tag selectNewTag = this.tagService.GetByTagName<Tag>("Nature Tag Name");
            Assert.Equal(newTadId, selectNewTag?.Id);
        }

        [Fact]
        public async void CreateTagOnlyWithName()
        {
            string newTadId = await this.tagService.CreateAsync("Name", null, null);
            Assert.NotNull(newTadId);
        }

        [Fact]
        public void GetTagByFullName()
        {
            Tag selectTag = this.tagService.GetByTagName<Tag>(TestTagName1);
            Assert.Equal(this.testTagId1, selectTag?.Id);
        }

        [Fact]
        public void GetTagByUndefineName()
        {
            Tag selectTag = this.tagService.GetByTagName<Tag>("undefine");
            Assert.Null(selectTag);
        }

        [Fact]
        public void FilteTagByFullName()
        {
            IEnumerable<Tag> selectTags = this.tagService.FiterTagsByNames<Tag>(TestTagName1);
            Assert.Single(selectTags);
        }

        [Fact]
        public void FilteTagByName()
        {
            IEnumerable<Tag> selectTags = this.tagService.FiterTagsByNames<Tag>(TestTagName2);
            Assert.Equal(selectTags.ToList()[0].Id, this.testTagId2);
        }

        [Fact]
        public void FilteTagByPartialName()
        {
            IEnumerable<Tag> selectTags = this.tagService.FiterTagsByNames<Tag>(TestTagPartialName);
            Assert.Equal(2, selectTags?.Count());
        }

        [Fact]
        public void FilteTagByUndefineName()
        {
            IEnumerable<Tag> selectTags = this.tagService.FiterTagsByNames<Tag>("undefine");
            Assert.Empty(selectTags);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.tagRepository.Dispose();
            }
        }

        private void InitTestServices()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString());

            ApplicationDbContext dbContext = new ApplicationDbContext(options.Options);

            this.tagRepository = new EfDeletableEntityRepository<Tag>(dbContext);

            this.tagService = new TagsService(
                this.tagRepository);
        }

        private async void AddTestData()
        {
            this.testTagId1 = await this.tagService.CreateAsync(TestTagName1, null, "userId1");
            this.testTagId2 = await this.tagService.CreateAsync(TestTagName2, null, "userId2");
        }
    }
}
