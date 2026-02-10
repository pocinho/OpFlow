// Copyright (c) 2026 Paulo Pocinho.

namespace PP.Toolkit.Operation.Tests;

public class MapTests
{
    // ------------------------------------------------------------
    //  Helpers
    // ------------------------------------------------------------

    private static Operation<int> Ok(int value)
        => Operation.Ok(value);

    private static Operation<int> Fail(string message)
        => Operation.Fail<int>(new Error.Unexpected(message));

    // ------------------------------------------------------------
    //  Map (sync)
    // ------------------------------------------------------------

    [Fact]
    public void Map_OnSuccess_MapsValue()
    {
        Operation<int> op = Ok(10);

        Operation<int> result = op.Map(x => x * 2);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(20, success.Result);
    }

    [Fact]
    public void Map_OnFailure_DoesNotInvokeMapper_PropagatesFailure()
    {
        Operation<int> op = Fail("ERR");
        bool mapperCalled = false;

        int Mapper(int x)
        {
            mapperCalled = true;
            return x * 2;
        }

        Operation<int> result = op.Map(Mapper);

        Assert.False(mapperCalled);

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("ERR", failure.Error.Message);
    }

    // ------------------------------------------------------------
    //  MapAsync
    // ------------------------------------------------------------

    [Fact]
    public async Task MapAsync_OnSuccess_MapsValue()
    {
        Operation<int> op = Ok(5);

        async Task<int> Mapper(int x)
        {
            await Task.Delay(1);
            return x + 3;
        }

        Operation<int> result = await op.MapAsync(Mapper);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(8, success.Result);
    }

    [Fact]
    public async Task MapAsync_OnFailure_DoesNotInvokeMapper_PropagatesFailure()
    {
        Operation<int> op = Fail("ERR-ASYNC");
        bool mapperCalled = false;

        async Task<int> Mapper(int x)
        {
            mapperCalled = true;
            await Task.Delay(1);
            return x + 3;
        }

        Operation<int> result = await op.MapAsync(Mapper);

        Assert.False(mapperCalled);

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("ERR-ASYNC", failure.Error.Message);
    }
}