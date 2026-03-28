using MusicStore.Application.Services;
using MusicStore.Domain.Entities;

namespace MusicStore.Tests;

public class OrderStatusWorkflowTests
{
    // --- Valid transitions ---

    [Fact]
    public void Pending_To_Confirmed_IsAllowed()
    {
        Assert.True(OrderStatusWorkflow.CanTransition(OrderStatus.Pending, OrderStatus.Confirmed));
    }

    [Fact]
    public void Pending_To_Cancelled_IsAllowed()
    {
        Assert.True(OrderStatusWorkflow.CanTransition(OrderStatus.Pending, OrderStatus.Cancelled));
    }

    [Fact]
    public void Confirmed_To_Shipped_IsAllowed()
    {
        Assert.True(OrderStatusWorkflow.CanTransition(OrderStatus.Confirmed, OrderStatus.Shipped));
    }

    [Fact]
    public void Confirmed_To_Cancelled_IsAllowed()
    {
        Assert.True(OrderStatusWorkflow.CanTransition(OrderStatus.Confirmed, OrderStatus.Cancelled));
    }

    [Fact]
    public void Shipped_To_Delivered_IsAllowed()
    {
        Assert.True(OrderStatusWorkflow.CanTransition(OrderStatus.Shipped, OrderStatus.Delivered));
    }

    // --- Invalid transitions ---

    [Fact]
    public void Pending_To_Shipped_IsNotAllowed()
    {
        Assert.False(OrderStatusWorkflow.CanTransition(OrderStatus.Pending, OrderStatus.Shipped));
    }

    [Fact]
    public void Pending_To_Delivered_IsNotAllowed()
    {
        Assert.False(OrderStatusWorkflow.CanTransition(OrderStatus.Pending, OrderStatus.Delivered));
    }

    [Fact]
    public void Confirmed_To_Delivered_IsNotAllowed()
    {
        Assert.False(OrderStatusWorkflow.CanTransition(OrderStatus.Confirmed, OrderStatus.Delivered));
    }

    [Fact]
    public void Confirmed_To_Pending_IsNotAllowed()
    {
        Assert.False(OrderStatusWorkflow.CanTransition(OrderStatus.Confirmed, OrderStatus.Pending));
    }

    [Fact]
    public void Shipped_To_Confirmed_IsNotAllowed()
    {
        Assert.False(OrderStatusWorkflow.CanTransition(OrderStatus.Shipped, OrderStatus.Confirmed));
    }

    [Fact]
    public void Shipped_To_Cancelled_IsNotAllowed()
    {
        Assert.False(OrderStatusWorkflow.CanTransition(OrderStatus.Shipped, OrderStatus.Cancelled));
    }

    [Fact]
    public void Delivered_To_AnyStatus_IsNotAllowed()
    {
        Assert.False(OrderStatusWorkflow.CanTransition(OrderStatus.Delivered, OrderStatus.Pending));
        Assert.False(OrderStatusWorkflow.CanTransition(OrderStatus.Delivered, OrderStatus.Confirmed));
        Assert.False(OrderStatusWorkflow.CanTransition(OrderStatus.Delivered, OrderStatus.Shipped));
        Assert.False(OrderStatusWorkflow.CanTransition(OrderStatus.Delivered, OrderStatus.Cancelled));
    }

    [Fact]
    public void Cancelled_To_AnyStatus_IsNotAllowed()
    {
        Assert.False(OrderStatusWorkflow.CanTransition(OrderStatus.Cancelled, OrderStatus.Pending));
        Assert.False(OrderStatusWorkflow.CanTransition(OrderStatus.Cancelled, OrderStatus.Confirmed));
        Assert.False(OrderStatusWorkflow.CanTransition(OrderStatus.Cancelled, OrderStatus.Shipped));
        Assert.False(OrderStatusWorkflow.CanTransition(OrderStatus.Cancelled, OrderStatus.Delivered));
    }

    // --- GetAllowedTransitions ---

    [Fact]
    public void GetAllowedTransitions_Pending_ReturnsConfirmedAndCancelled()
    {
        var allowed = OrderStatusWorkflow.GetAllowedTransitions(OrderStatus.Pending);
        Assert.Contains("Confirmed", allowed);
        Assert.Contains("Cancelled", allowed);
        Assert.Equal(2, allowed.Length);
    }

    [Fact]
    public void GetAllowedTransitions_Delivered_ReturnsEmpty()
    {
        var allowed = OrderStatusWorkflow.GetAllowedTransitions(OrderStatus.Delivered);
        Assert.Empty(allowed);
    }

    [Fact]
    public void GetAllowedTransitions_Cancelled_ReturnsEmpty()
    {
        var allowed = OrderStatusWorkflow.GetAllowedTransitions(OrderStatus.Cancelled);
        Assert.Empty(allowed);
    }
}
