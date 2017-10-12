#pragma once
class pixel
{
	int r, g, b; // Couleurs

public:
	pixel( int r, int g, int b );
	pixel();

	void set(int r, int g, int b);
	void setR(int r);
	void setG(int g);
	void setB(int b);
	int getR();
	int getG();
	int getB();

	~pixel();
};