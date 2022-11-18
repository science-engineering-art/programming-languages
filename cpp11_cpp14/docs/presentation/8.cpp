
template<typename R,typename... T>
using Function = R(*)(T...args);

template<typename R>
    static linked_list<R> Map(linked_list<T> lst, Function<R,T> f)
    {
        linked_list<R> out = linked_list<R>();
        for (int i = 0; i < lst.size; i++)
        {
            out.Add_Last(f(lst[i]));
        }
        return out;
        
    }