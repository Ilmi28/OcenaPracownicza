import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import Button from '@mui/material/Button';

import axiosClient from "./services/axiosClient";

function App() {
  const [count, setCount] = useState(0)


  const testLogin = async () => {
    try {
      await axiosClient.post("/auth/login", {
        username: "admin",
        password: "admin123"
      });
      console.log("LOGIN OK");
    } catch (e) {
      console.log("LOGIN ERROR", e);
    }
  };

  const testSecure = async () => {
    try {
      const res = await axiosClient.get("/auth/secure");
      console.log("SECURE OK:", res.data);
    } catch (e) {
      console.log("SECURE ERROR:", e);
    }
  };

  return (
    <>
      <div style={{ padding: '20px' }}>
        <h1>Test MUI Button</h1>
        <Button variant="contained">Hello world</Button>
      </div>
      <div>
        <a href="https://vite.dev" target="_blank">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a href="https://react.dev" target="_blank">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>
      <h1>Vite + React</h1>
      <div className="card">
        <button onClick={() => setCount((count) => count + 1)}>
          count is {count}
        </button>
        <p>
          Edit <code>src/App.tsx</code> and save to test HMR
        </p>
      </div>
      <p className="read-the-docs">
        Click on the Vite and React logos to learn more
      </p>
      <div style={{ padding: 20 }}>
        <button onClick={testLogin}>Test Login</button>
        <button onClick={testSecure}>Test Secure</button>
    </div>
    </>
  )
}

export default App
