'use client'

import { useState, ChangeEvent } from 'react';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';

interface FileType {
  fileName: string;
  status: string;
}

export default function Home() {
  const [files, setFiles] = useState<FileType[]>([]);
  const [token, setToken] = useState<string | null>(null);
  const [isRegistering, setIsRegistering] = useState(false);

  const handleAuth = async (values: { username: string; password: string }) => {
    const endpoint = isRegistering ? 'register' : 'login';

    try {
      const response = await fetch(`http://localhost:9010/api/${endpoint}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(values),
      });

      if (response.ok) {
        const data = await response.json();
        if (!isRegistering) {
          setToken(data.token);
          localStorage.setItem('token', data.token);
        } else {
          alert('Registro exitoso. Ahora inicia sesión.');
          setIsRegistering(false);
        }
      } else {
        alert(`Error en ${isRegistering ? 'registro' : 'login'}`);
      }
    } catch (error) {
      console.error(`Error en ${isRegistering ? 'registro' : 'login'}:`, error);
    }
  };

  const handleFileUpload = async (event: ChangeEvent<HTMLInputElement>) => {
    if (!token) return alert('Debes iniciar sesión');

    const file = event.target.files?.[0];
    if (!file) return;

    const formData = new FormData();
    formData.append('file', file);

    try {
      const response = await fetch('http://localhost:9010/api/upload', {
        method: 'POST',
        headers: { Authorization: `Bearer ${token}` },
        body: formData,
      });

      if (response.ok) {
        alert('Archivo subido correctamente');
        fetchFiles();
      }
    } catch (error) {
      console.error('Error al subir archivo:', error);
    }
  };

  const fetchFiles = async () => {
    if (!token) return;

    try {
      const response = await fetch('http://localhost:9010/api/files', {
        headers: { Authorization: `Bearer ${token}` },
      });
      const data: FileType[] = await response.json();
      setFiles(data);
    } catch (error) {
      console.error('Error obteniendo archivos:', error);
    }
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-100 p-4">
      <div className="w-full max-w-md bg-white shadow-lg rounded-lg p-6">
        {!token ? (
          <div>
            <h2 className="text-2xl font-semibold text-black text-center mb-4">
              {isRegistering ? 'Registro' : 'Iniciar Sesión'}
            </h2>
            <Formik
              initialValues={{ username: '', password: '' }}
              validationSchema={Yup.object({
                username: Yup.string().email('Email inválido').required('El email es obligatorio'),
                password: Yup.string().required('La contraseña es obligatoria'),
              })}
              onSubmit={handleAuth}
            >
              <Form className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700">Email</label>
                  <Field
                    type="email"
                    name="username"
                    className="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  />
                  <ErrorMessage name="email" component="p" className="text-red-500 text-sm mt-1" />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Contraseña</label>
                  <Field
                    type="password"
                    name="password"
                    className="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  />
                  <ErrorMessage name="password" component="p" className="text-red-500 text-sm mt-1" />
                </div>

                <button
                  type="submit"
                  className="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 rounded-lg transition"
                >
                  {isRegistering ? 'Registrarse' : 'Iniciar Sesión'}
                </button>
              </Form>
            </Formik>

            <p className="text-center mt-4 text-sm">
              {isRegistering ? '¿Ya tienes una cuenta?' : '¿No tienes cuenta?'}
              <button
                onClick={() => setIsRegistering(!isRegistering)}
                className="ml-2 text-blue-600 hover:underline"
              >
                {isRegistering ? 'Inicia sesión' : 'Regístrate'}
              </button>
            </p>
          </div>
        ) : (
          <div>
            <h2 className="text-2xl font-semibold text-gray-700 text-center mb-4">Subida de Archivos</h2>
            <input
              type="file"
              onChange={handleFileUpload}
              className="w-full border text-black rounded-lg px-3 py-2"
            />
            <button
              onClick={fetchFiles}
              className="mt-4 w-full bg-green-600 hover:bg-green-700 text-white font-semibold py-2 rounded-lg transition"
            >
              Cargar archivos
            </button>

            <ul className="mt-4 space-y-2">
              {files.map((file, index) => (
                <li key={index} className="p-2 bg-gray-200 text-black rounded-lg">
                  {file.fileName} - {file.status}
                </li>
              ))}
            </ul>
          </div>
        )}
      </div>
    </div>
  );
}