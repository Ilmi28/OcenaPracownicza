import { jsx as _jsx, jsxs as _jsxs, Fragment as _Fragment } from "react/jsx-runtime";
import { useState } from 'react';
import reactLogo from './assets/react.svg';
import viteLogo from '/vite.svg';
import './App.css';
import Button from '@mui/material/Button';
import axiosClient from "./services/axiosClient";
function App() {
    const [count, setCount] = useState(0);
    const testLogin = async () => {
        try {
            await axiosClient.post("/auth/login", {
                username: "admin",
                password: "admin123"
            });
            console.log("LOGIN OK");
        }
        catch (e) {
            console.log("LOGIN ERROR", e);
        }
    };
    const testSecure = async () => {
        try {
            const res = await axiosClient.get("/auth/secure");
            console.log("SECURE OK:", res.data);
        }
        catch (e) {
            console.log("SECURE ERROR:", e);
        }
    };
    return (_jsxs(_Fragment, { children: [_jsxs("div", { style: { padding: '20px' }, children: [_jsx("h1", { children: "Test MUI Button" }), _jsx(Button, { variant: "contained", children: "Hello world" })] }), _jsxs("div", { children: [_jsx("a", { href: "https://vite.dev", target: "_blank", children: _jsx("img", { src: viteLogo, className: "logo", alt: "Vite logo" }) }), _jsx("a", { href: "https://react.dev", target: "_blank", children: _jsx("img", { src: reactLogo, className: "logo react", alt: "React logo" }) })] }), _jsx("h1", { children: "Vite + React" }), _jsxs("div", { className: "card", children: [_jsxs("button", { onClick: () => setCount((count) => count + 1), children: ["count is ", count] }), _jsxs("p", { children: ["Edit ", _jsx("code", { children: "src/App.tsx" }), " and save to test HMR"] })] }), _jsx("p", { className: "read-the-docs", children: "Click on the Vite and React logos to learn more" }), _jsxs("div", { style: { padding: 20 }, children: [_jsx("button", { onClick: testLogin, children: "Test Login" }), _jsx("button", { onClick: testSecure, children: "Test Secure" })] })] }));
}
export default App;
