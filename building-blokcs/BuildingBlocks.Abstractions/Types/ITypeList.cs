﻿namespace BuildingBlocks.Abstractions.Types
{
    public interface ITypeList : ITypeList<object> { }
    public interface ITypeList<in TBaseType> : IList<Type>
    {
        void Add<T>() where T : TBaseType;

        bool TryAdd<T>() where T : TBaseType;

        bool Contains<T>() where T : TBaseType;

        void Remove<T>() where T : TBaseType;
    }
}
