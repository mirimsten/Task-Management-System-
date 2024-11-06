﻿namespace DalApi;

public interface ICrud<T> where T : class
{
    int Create(T item);
    T? Read(int id);
    T? Read(Func<T, bool> filter); // stage 2
    IEnumerable<T> ReadAll(Func<T, bool>? filter = null);
    void Update(T item);
    void Delete(int id);
    void Reset();
}

