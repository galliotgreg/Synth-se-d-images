from math import *
class Vec3 : #classe représentant des vecteurs 3D qui servira à manipuler les vecteurs et les points
	"""
	Attributs : x,y,z
	Constructeurs
	Accesseurs(get,set)
	Opérateurs d'addition, de soustraction, de multiplication et de division entre vec3 et multi et div avec un float
	Fonction Norme renvoyant la norme d'un vecteurs
	Fonction Norme au carré renvoyant la norme au carré d'un vecteurs
	Foncion Normalized renvoyant la version normalisée d'un vecteurs
	Fonction produit scalaire entre deux vecteurs
	Fonction produit vectoriel entre deux vecteurs
	Fonction d'affichage d'un vecteur
	"""
	
	# Constructeurs, todo : à voir le polymorphisme avec python
	"""def __init__(self) : 
		self.x = 0
		self.y = 0
		self.z = 0"""
	def __init__(self, x, y, z):
		self.x = x
		self.y = y
		self.z = z
	
	# Accesseurs
	def _get_x(self) : 
		return self.x	
	def _get_y(self) : 
		return self.y
	def _get_z(self) : 
		return self.z
	
	def _set_x(self, a):
		self.x = a
	def _set_y(self, a):
		self.y = a
	def _set_z(self, a):
		self.z = a
	
	# Opérateurs
	def _addition_(vect1, vect2) :
		return Vec3(vect1.x+vect2.x,vect1.y+vect2.y,vect1.z+vect2.z)
	
	def _soustraction_(vect1,vect2) :
		return Vec3(vect1.x-vect2.x,vect1.y-vect2.y,vect1.z-vect2.z)
		
	def _multiplication_(vect1,vect2) :
		return Vec3(vect1.x*vect2.x,vect1.y*vect2.y,vect1.z*vect2.z)
	
	def _multiplication_float_(vect, a) : 
		return Vec3(vect.x*a, vect.y*a, vect.z*a)
		
	def _division_(vect1,vect2) :
		if(vect2.x != 0):
			if (vect2.y != 0):
				if (vect2.z != 0):
					return Vec3(vect1.x/vect2.x,vect1.y/vect2.y,vect1.z/vect2.z)
		print("Division par zéro")
		return None
	
	def _division_float_(vect, a) :
		if(a!=0):
			return Vec3(vect.x/a, vect.y/a, vect.z/a)
		print("Division par 0")
		return None 
	
	def _norme_(self) :
		return sqrt(self.x*self.x + self.y*self.y + self.z*self.z)
	
	def _norme_au_carre_(self) :
		norme = self._norme_()
		return norme*norme
		
	def _normalized_(self) :
		norme = self._norme_()
		if(norme!=0):
			return self._division_float_(norme)
		print("Division par 0")
		return None
	
	def _produit_scalaire_(vect1,vect2):
		return vect1.x*vect2.x + vect1.y*vect2.y + vect1.z*vect2.z
	
	def _produit_vectoriel_(vect1,vect2):
		x = vect1.y*vect2.z - vect1.z*vect2.y
		y = vect1.z*vect2.x - vect1.x*vect2.z
		z = vect1.x*vect2.y - vect1.y*vect2.x
		vecRes = Vec3(x,y,z)
		return vecRes
	
	# fonction d'affichage d'un vecteur sous la forme (x,y,z)
	def _print_vect(self):
		print("(",self.x,", ",self.y,", ",self.z,")")
		
def main():
	vec1 = Vec3(1,2,3)
	vec2 = Vec3(3,2,1)
	
	vec3 = vec1._addition_(vec2)
	print("addition")
	vec3._print_vect() # On attend le résultat suivant: (4,4,4)
	
	vec3 = vec2._soustraction_(vec1)
	print("soustraction")
	vec3._print_vect() # On attend le résultat suivant : (2,0,-2)
	
	vec3 = vec1._multiplication_(vec2)
	print("multiplication")
	vec3._print_vect() # On attend le résultat suivant : (3,4,3)
	
	vec3 = vec1._multiplication_float_(3)
	print("multiplication avec float")
	vec3._print_vect() # On attend le résultat suivant : (3,6,9)
	
	vec3 = vec1._division_(vec2)
	print("division")
	vec3._print_vect() # On attend le résultat suivant : (0.33333333,1,3)
	
	vec3 = vec1._division_float_(3)
	print("division avec float")
	vec3._print_vect() # On attend le résultat suivant : (0.33333333,0.666666666,1)
	
	norme = vec1._norme_()
	print("norme")
	print(str(norme)) # On attend le résultat suivant :3.7416573867739413
	
	norme_carre = vec1._norme_au_carre_()
	print("norme au carre")
	print(str(norme_carre)) # On attend le résultat suivant : 14
	
	vec3=vec1._normalized_()
	print("normalisé")
	vec3._print_vect() # On attend le résultat suivant : ( 0.2672612419124244 ,  0.5345224838248488 ,  0.8017837257372732 )
	
	produit_scalaire = vec1._produit_scalaire_(vec2)
	print("produit scalaire")
	print(str(produit_scalaire)) # On attend le résultat suivant : 10
	
	vec3 = vec1._produit_vectoriel_(vec2)
	print("produit vectoriel")
	vec3._print_vect() # On attend le résultat suivant :(-4,8,-4)