---
author: Winand, Roald, Sjoerd, Alexey and Anvar
title: Concepts of programming languages
subtitle: Prolog
theme: uucs
mainfont: Ubuntu Light
sansfont: Ubuntu Light
---


# What is logic programming?
Logic programming is a type of programming paradigm which is largely based on formal logic.Any program written in a logic programming language is a set of sentences in logical form, expressing facts and rules about some problem domain. In logic programing, rules are written in the form of clauses:

H :- B1, …, Bn.

and are read declaratively as logical implications:

H if B1 and … and Bn.

H - head of the rule; B1, …, Bn - the body; H. - facts


# What is Prolog?

Prolog(PROgramming in LOGic) is a logic programming language that allows us to "program" with declarative knowledge.The language was first conceived by a group around Alain Colmerauer in Marseille, France, in the early 1970s and the first Prolog system was developed in 1972 by Colmerauer with Philippe Roussel.It was developed from a foundation of logical theorem proving and originally used for research in natural language processing.

# What is Prolog?

* A general-purpose logic programming language.
* One of the first and most popular logic programming language available.
* Originally intended as a way to process natural language.

SWI Prolog (http://www.swi-prolog.org/) one of the most mature implementations of Prolog.

# Application of Prolog

Prolog is still being used nowadays in various industrial, medical & commercial areas to:

* build expert systems that solve complex problems without the help of humans (e.g. automatically planning, monitoring, controlling and troubleshooting complex systems)
* build decision support systems that aid organizations in decision-making (e.g. decision systems for medical diagnoses)
* online support service for customers, etc.

# Knowledge database
* Prolog programs have two parts: a database (of facts and rules), and an interactive "query" tool.
* Prolog databases are "consulted" (loaded), and then the query tool is used to "make queries" (ask questions) about the database.
* How queries are answered is generally beyond the control of the programmer; Prolog uses a depth-first search to figure out how to answer queries.
* "Programs" written in Prolog are "executed" by performing queries.