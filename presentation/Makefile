
default:slides.pdf

verbose:slides.md
	pandoc -t beamer slides.md -o slides.pdf --latex-engine=xelatex --verbose --template beamer-template.tex


slides.pdf:slides.md
	pandoc -t beamer slides.md -o slides.pdf --latex-engine=xelatex --template beamer-template.tex


winand:winand.md
	pandoc -t beamer winand.md -o winand.pdf --latex-engine=xelatex --template beamer-template.tex

sjoerd:SlidesSjoerd.md
	pandoc -t beamer SlidesSjoerd.md -o sjoerd.pdf --latex-engine=xelatex --template beamer-template.tex

