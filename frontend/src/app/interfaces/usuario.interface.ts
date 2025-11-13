export interface Usuario {
  id: number;
  login: string;
  nomeCompleto: string;
  email: string;
  acessos: number;
}

export interface LoginDTO {
  login: string;
  senha: string;
}

export interface UsuarioCreateDTO {
  login: string;
  nomeCompleto: string;
  email: string;
  senha: string;
}