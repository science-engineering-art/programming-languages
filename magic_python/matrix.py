import re
from typing import Any, List, Tuple, Iterator, Generic, TypeVar


class Meta(type):
    def __new__(cls, name, bases, dct):
        print('__new__ from Metaclass')
        print(cls, name, bases, dct)
        return super().__new__(cls, name, bases, dct)


T = TypeVar('T')


class Matrix:

    def __new__(cls, *args, **kwargs):
        new_object = object.__new__(cls)
        return new_object

    def __init__(self, rows: int, cols: int, init_value=None):
        self.amount_rows: int = rows
        self.amount_cols: int = cols
        self.matrix: List[List[T]] = [[init_value
                                       for _ in range(0, cols)]
                                      for _ in range(0, rows)]

    def __eq__(self, other: 'Matrix[int, int]') -> bool:
        if self.amount_cols != other.amount_cols or self.amount_rows != other.amount_rows:
            return False

        for i in range(0, self.amount_rows):
            for j in range(0, self.amount_cols):
                if self[i, j] != other[i, j]:
                    return False

        return True

    def __add__(self, other: 'Matrix[T]') -> 'Matrix[T]':
        """
            Sumador de matrices.
        """

        result = Matrix(rows=self.amount_rows,
                        cols=self.amount_cols, init_value=None)

        for i in range(0, self.amount_rows):
            for j in range(0, self.amount_cols):
                result[i, j] = self[i, j] + other[i, j]

        return result

    def __mul__(self, other: 'Matrix[T]') -> 'Matrix[T]':
        """
            Multiplicador de matrices.
        """

        if self.amount_cols != other.amount_rows:
            raise Exception('Invalid operation.')

        result = Matrix(rows=self.amount_rows,
                        cols=other.amount_cols, init_value=None)

        for i in range(0, self.amount_rows):
            for j in range(0, other.amount_cols):
                for h in range(0, self.amount_cols):
                    if h == 0:
                        result[i, j] = self[i, h] * other[h, j]
                    else:
                        result[i, j] += self[i, h] * other[h, j]

        return result

    # hay que heredar de `object` para usar funciones mágicas

    def __getitem__(self, key: Tuple[int, int]):
        """
            Indizador con una sintaxis más cómoda. 
            Ejemplo: a = matrix[i,j]
        """

        if not isinstance(key, tuple):
            raise Exception('Format incorrect.')

        if len(key) != 2:
            raise Exception('Number of parameters exceded.')
        i, j = key

        if i >= 0 and i < self.amount_rows and \
                j >= 0 and j < self.amount_cols:
            return self.matrix[i][j]
        else:
            raise Exception('Index out of matrix.')

    def __setitem__(self, key: Tuple[int, int], value: T):
        """
            Permite setear el valor de la matriz indexada, 
            de manera más cómoda.
            Ejemplo: `matrix[i,j] = 4`
        """

        if not isinstance(key, tuple):
            raise Exception('Format incorrect.')

        if len(key) != 2:
            raise Exception('Number of parameters exceded.')
        i, j = key

        if i >= 0 and i < self.amount_rows and \
                j >= 0 and j < self.amount_cols:
            self.matrix[i][j] = value
        else:
            raise Exception('Index out of matrix.')

    def __getattr__(self, __name: str):
        matched = re.match(r"_(\d+)_(\d+)", __name)
        if matched:
            i, j = matched.groups()
            i = int(i)
            j = int(j)
            return self[i, j]

        matched = re.match(r"as_([a-z]+)", __name)
        if matched:
            type = matched.groups()[0]
            result = Matrix(self.amount_rows, self.amount_cols)

            for i in range(0, self.amount_rows):
                for j in range(0, self.amount_cols):
                    result[i, j] = eval(f'{type}(self.matrix[i][j])')

            return lambda: result

    def __setattr__(self, __name: str, __value: Any):
        matched = re.match(r"_(\d+)_(\d+)", __name)
        if matched:
            i, j = matched.groups()
            i = int(i)
            j = int(j)
            self[i, j] = __value
        return super().__setattr__(__name, __value)

    def __getattribute__(self, __name: str) -> Any:
        return super().__getattribute__(__name)

    def __next__(self):
        for row in self.matrix:
            for i in row:
                yield i

    def __iter__(self):
        return self.__next__()

    def __repr__(self) -> str:
        result = ''
        j = 0
        for i in self:
            j += 1
            result += f'{i} '
            if j % self.amount_cols == 0 and j != \
                    self.amount_cols * self.amount_rows:
                result += '\n'
        return result

    def __len__(self) -> int:
        return self.amount_cols * self.amount_rows

    def __call__(self, *args: Any, **kwds: Any) -> Any:
        print(f"I'm callable.. args:{args} kwds:{kwds}")

# matrixs = []

# for i in range(0,10):
#     matrixs.append(Matrix(2,3, init_value=i))
#     print('not yet')

# print(matrixs[2].__dict__)

# matrixs[0]._0_1 = 3
# print(matrixs[0])

# Examples


a = Matrix(2, 3, init_value=0)
b = Matrix(2, 3, init_value=1)

# print('a + b')
# print(f'{a+b}\n')

# print(a.__dict__)
# print(a.__class__)

# print("a.__getattribute__('__add__')(b)")
# print(f"{a.__getattribute__('__add__')(b)}\n")
# print(f"{a.__class__.__dict__['__add__'](a,b)}\n")
# print(a.__class__.__dict__['__add__'].__getattribute__('__call__')(a, b))

# print(a())

# print("Matrix.__add__.__getattribute__('__call__')(a,b)")
# print(Matrix.__add__.__getattribute__('__call__')(a, b))

# print(a.__class__.__getattribute__['__add__'].__getattribute__('__call__')(a,b))

# x = 1
# eval('print(x)')
