#include "stdafx.h"
#include "pixel.h"

pixel::pixel(int r, int g, int b){
	this->set( r, g, b );
}
pixel::pixel() {
	this->set( 0, 0, 0 );
}

void pixel::set(int r, int g, int b) {
	this->setR(r);
	this->setB(b);
	this->setG(g);
}
void pixel::setR(int r) {
	this->r = r;
}
void pixel::setG(int g) {
	this->g = g;
}
void pixel::setB(int b) {
	this->b = b;
}
int pixel::getR() {
	return this->r;
}
int pixel::getG() {
	return this->g;
}
int pixel::getB() {
	return this->b;
}

pixel::~pixel() {}