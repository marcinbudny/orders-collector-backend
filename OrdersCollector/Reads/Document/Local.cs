using System.Collections.Generic;
using System.Threading.Tasks;
using Marten.Events.Projections;
using OrdersCollector.Domain.Local.Events.V1;

namespace OrdersCollector.Reads.Document
{
    public class Local
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List<string> Aliases { get; set; } = new List<string>();
    }

    public class LocalProjection : ViewProjection<Local, string>
    {
        public LocalProjection() 
        {
            ProjectEventAsync<LocalAdded>(e => e.Id, Apply);
            ProjectEventAsync<LocalRenamed>(e => e.Id, Apply);
            ProjectEventAsync<LocalAliasAdded>(e => e.LocalId, Apply);
            ProjectEventAsync<LocalAliasRemoved>(e => e.LocalId, Apply);
            DeleteEvent<LocalRemoved>(e => e.Id);
        }

        public Task Apply(Local view, LocalAdded e)
        {
            view.Id = e.Id;
            view.Name = e.Name;
            return Task.CompletedTask;
        }

        public Task Apply(Local view, LocalRenamed e)
        {
            view.Name = e.NewName;
            return Task.CompletedTask;
        }

        public Task Apply(Local view, LocalAliasAdded e)
        {
            if(!view.Aliases.Contains(e.Alias))
                view.Aliases.Add(e.Alias);
            return Task.CompletedTask;
        }

        public Task Apply(Local view, LocalAliasRemoved e)
        {
            if (view.Aliases.Contains(e.Alias))
                view.Aliases.Remove(e.Alias);
            return Task.CompletedTask;
        }

    }
}