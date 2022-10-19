linked_list(initializer_list<T> lst)
        {
            size = 0;
            head = tail = nullptr;
            for(auto it = lst.begin();it!=lst.end();++it)
            {
                this->add_element(*it);
            }
        }
        