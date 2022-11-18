linked_list(vector<T> & v)
        {
            size = 0;
            head = tail = nullptr;
            for_each(v.begin(),v.end(),[&](T x){this->Add_Last(x);});
        }