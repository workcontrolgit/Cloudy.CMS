﻿using Cloudy.CMS.ContentTypeSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Cloudy.CMS.ContentSupport.Serialization;
using Cloudy.CMS.DocumentSupport;
using MongoDB.Driver;

namespace Cloudy.CMS.ContentSupport.RepositorySupport
{
    public class ContentUpdater : IContentUpdater
    {
        IContainerProvider ContainerProvider { get; }
        IContentTypeProvider ContentTypeRepository { get; }
        string Container { get; } = "content";
        IContentSerializer ContentSerializer { get; }

        public ContentUpdater(IContainerProvider containerProvider, IContentTypeProvider contentTypeRepository, IContentSerializer contentSerializer)
        {
            ContainerProvider = containerProvider;
            ContentTypeRepository = contentTypeRepository;
            ContentSerializer = contentSerializer;
        }

        public void Update(IContent content)
        {
            if (content.Id == null)
            {
                throw new InvalidOperationException($"This content cannot be updated as it doesn't seem to exist (Id is null). Did you mean to use IContentCreator?");
            }

            var contentType = ContentTypeRepository.Get(content.ContentTypeId);

            if (contentType == null)
            {
                throw new InvalidOperationException($"This content has no content type (or rather its base class has no [ContentType] attribute)");
            }

            var document = ContentSerializer.Serialize(content, contentType);

            ContainerProvider.Get(ContainerConstants.Content).UpdateOne(Builders<Document>.Filter.Eq(d => d.Id, content.Id), Builders<Document>.Update.Set(d => d.GlobalFacet, document.GlobalFacet));
        }

        public async Task UpdateAsync(IContent content)
        {
            if (content.Id == null)
            {
                throw new InvalidOperationException($"This content cannot be updated as it doesn't seem to exist (Id is null). Did you mean to use IContentCreator?");
            }

            var contentType = ContentTypeRepository.Get(content.ContentTypeId);

            if (contentType == null)
            {
                throw new InvalidOperationException($"This content has no content type (or rather its base class has no [ContentType] attribute)");
            }

            var document = ContentSerializer.Serialize(content, contentType);

            await ContainerProvider.Get(ContainerConstants.Content).UpdateOneAsync(Builders<Document>.Filter.Eq(d => d.Id, content.Id), Builders<Document>.Update.Set(d => d.GlobalFacet, document.GlobalFacet));
        }
    }
}
