---
author: Winand, Roald, Sjoerd, Alexey and Anvar
title: Concepts of programming languages
subtitle: Prolog
theme: uucs
mainfont: Ubuntu Light
sansfont: Ubuntu Light
---

# Cut
Take this simple knowledge base:
```prolog
bar(a).
bar(b).
baz(a).
baz(b).

foo(X, Y) :- bar(X), baz(Y).
```

which translates to the tree:

![Branches of foo(X, Y)](img/latex/cut1.pdf "Alt caption"){ width=100% }

# Cut 2

![Branches of foo(X, Y)](img/latex/cut1.pdf "Alt caption"){ width=100% }

Querying 'foo(X, Y)' gives the result: 

(X, Y) = {(a, a), (a, b), (b, a), (b, b)}

But what if you would only like to get a partial result? Use a cut!

# Cut 3

A cut is an atom `!' that can be used to 

```
foo(X, Y) :- bar(X), !, baz(Y).
```
The query 'foo(X, Y)' now given the results: (

X = Y, Y = a ;
X = a,
Y = b.
