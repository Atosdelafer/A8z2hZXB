female(anne).
female(esther).
female(mildred).

parent(esther,rosie).
parent(esther,dicky).
parent(rosie,randy).

mother(X,Y) :- female(X),parent(X,Y).
