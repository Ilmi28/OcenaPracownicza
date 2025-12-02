import "./App.css";
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import MainLayout from './layouts/MainLayout';
import Dashboard from './pages/Dashboard';
import Users from './pages/Users';
import Settings from './pages/Settings';

function App() {

    return (
        <>
            <BrowserRouter>
            <Routes>
                <Route path="/" element={<MainLayout />}>
                <Route index element={<Dashboard />} />
                <Route path="users" element={<Users />} />
                <Route path="settings" element={<Settings />} />
                </Route>
            </Routes>
            </BrowserRouter>
        </>
    );
}

export default App;
