using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using OrdersCollector.Utils;

namespace OrdersCollector.Tests.Unit
{
    public static class TestAggregateEntryPoint
    {
        public static TestAggregate<T> Test<T>()
            where T : AggregateRoot<string>, new()
        {
            return new TestAggregate<T>();
        }
    }
    
    
    public class TestAggregate<T> 
        where T : AggregateRoot<string>, new()
    {
        private T _aggregate;
        private Action<T> _testAction;

        public TestAggregate() => _aggregate = new T();

        public TestAggregate<T> Given(params object[] eventHistory)
        {
            _aggregate.ReplayAll(eventHistory, versionAfterReplay: 0);
            return this;
        }
        
        public TestAggregate<T> WhenCreated(Func<T> createFunc)
        {
            _aggregate = createFunc();
            return this;
        }

        public TestAggregate<T> When(Action<T> testAction)
        {
            _testAction = testAction;
            return this;
        }

        public void Expect(params object[] publishedEvents)
        {
            _testAction(_aggregate);
            _aggregate.PublishedEvents.Should().BeEquivalentTo(publishedEvents);
        }

        public void Expect(Action<IEnumerable<object>> evaluateEvents)
        {
            _testAction(_aggregate);
            evaluateEvents(_aggregate.PublishedEvents);
        }

        public void Expect<TEvent>(Action<TEvent> evaluateEvent)
        {
            _testAction(_aggregate);
            _aggregate.PublishedEvents.Count.Should().Be(1, because: "there should be 1 and only event");
            _aggregate.PublishedEvents.First().Should().BeOfType<TEvent>(because: "event should have expected type");
            evaluateEvent((TEvent)_aggregate.PublishedEvents.First());
        }

        public void Expect<TException>() where TException : Exception
        {
            Action boundAction = () => _testAction(_aggregate);
            boundAction.Should().Throw<TException>();
        }

        public void ExpectThrows<TException>(Expression<Func<TException, bool>> where) 
            where TException : Exception
        {
            Action boundAction = () => _testAction(_aggregate);
            boundAction.Should().Throw<TException>().Where(where);
        }
    }
}