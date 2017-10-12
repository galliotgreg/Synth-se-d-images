def clamp(x,min,max):
	if x < min :
		return min
	elif x > max:
		return max
	else :
		return x

def save_img(w,h):
	fichier = open("image.ppm", "w")
	fichier.write("P3\n")
	fichier.write(str(w))
	fichier.write(" ")
	fichier.write(str(h))
	fichier.write("\n")
	fichier.write("255\n")
	r = 0
	g = 0
	b = 0 
	for i in range(0,h):
		for j in range(0,w):
			#fichier.write( "221 152 92 \n" )
			fichier.write(str(r))
			fichier.write(" ")
			fichier.write(str(g))
			fichier.write(" ")
			fichier.write(str(b))
			fichier.write(" \n")
	fichier.close()
	
