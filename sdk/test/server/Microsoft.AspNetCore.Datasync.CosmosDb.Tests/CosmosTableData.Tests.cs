// Copyright (c) Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License.

namespace Microsoft.AspNetCore.Datasync.CosmosDb.Tests;

[ExcludeFromCodeCoverage]
public class CosmosTableData_Tests
{
    [Theory, ClassData(typeof(ITableData_TestData))]
    public void CosmosEntityTableData_Equals(ITableData a, ITableData b, bool expected)
    {
        CosmosTableData entity_a = a.ToTableEntity<CosmosTableData>();
        CosmosTableData entity_b = b.ToTableEntity<CosmosTableData>();

        entity_a.Equals(entity_b).Should().Be(expected);
        entity_b.Equals(entity_a).Should().Be(expected);

        entity_a.Equals(null).Should().BeFalse();
        entity_b.Equals(null).Should().BeFalse();
    }
}