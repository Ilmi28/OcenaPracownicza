import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import axiosClient from "../services/axiosClient";

export interface AchievementListItem {
    id: string;
    name: string;
    description: string;
    date: string;
    category: number;
    period: string;
    finalScore: string;
    achievementsSummary: string;
    stage2Status: number;
    stage2Comment?: string | null;
    employeeId: string;
    createdAt: string;
}

const KATEGORIE: Record<number, { nazwa: string; kolor: string }> = {
    1: { nazwa: "Sukces projektowy", kolor: "#007bff" },
    2: { nazwa: "Rozwój techniczny", kolor: "#6f42c1" },
    3: { nazwa: "Innowacja", kolor: "#28a745" },
    4: { nazwa: "Mentoring", kolor: "#fd7e14" },
    5: { nazwa: "Inne", kolor: "#6c757d" },
};

const AchievementList: React.FC = () => {
    const [achievements, setAchievements] = useState<AchievementListItem[]>([]);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    const fetchAchievements = async () => {
        setLoading(true);
        try {
            const resp =
                await axiosClient.get<AchievementListItem[]>("/achievement");
            setAchievements(resp.data);
        } catch (err: any) {
            console.error("Błąd pobierania danych.", err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchAchievements();
    }, []);

    if (loading) return <div className="loader-table">Ładowanie danych...</div>;

    return (
        <div className="table-container">
            <header className="table-header">
                <div>
                    <h1>Lista Osiągnięć</h1>
                    <span className="count-tag">
                        {achievements.length} pozycji
                    </span>
                </div>
                <div className="table-actions">
                    <button onClick={fetchAchievements} className="btn-refresh">
                        Odśwież
                    </button>
                    <button
                        onClick={() => navigate("/achievement/add")}
                        className="btn-add-primary"
                    >
                        + Dodaj osiągnięcie
                    </button>
                </div>
            </header>

            <div className="responsive-table-wrapper">
                <table className="achievement-table">
                    <thead>
                        <tr>
                            <th>Data</th>
                            <th>Kategoria</th>
                            <th>Okres</th>
                            <th>Wynik</th>
                            <th>Nazwa</th>
                            <th>Opis</th>
                            <th>Podsumowanie</th>
                            <th>Pracownik</th>
                            <th style={{ textAlign: "right" }}>Akcje</th>
                        </tr>
                    </thead>
                    <tbody>
                        {achievements.map((item) => {
                            const kat =
                                KATEGORIE[item.category] || KATEGORIE[5];
                            return (
                                <tr key={item.id}>
                                    <td className="col-date">
                                        {new Date(item.date).toLocaleDateString(
                                            "pl-PL",
                                        )}
                                    </td>
                                    <td>
                                        <span
                                            className="cat-badge"
                                            style={{
                                                borderColor: kat.kolor,
                                                color: kat.kolor,
                                            }}
                                        >
                                            {kat.nazwa}
                                        </span>
                                    </td>
                                    <td>{item.period}</td>
                                    <td>{item.finalScore}</td>
                                    <td className="col-name">{item.name}</td>
                                    <td
                                        className="col-desc"
                                        title={item.description}
                                    >
                                        {item.description}
                                    </td>
                                    <td
                                        className="col-desc"
                                        title={item.achievementsSummary}
                                    >
                                        {item.achievementsSummary}
                                    </td>
                                    <td className="col-emp">
                                        <code>
                                            {item.employeeId.slice(0, 8)}
                                        </code>
                                    </td>
                                    <td style={{ textAlign: "right" }}>
                                        <button
                                            className="btn-view"
                                            onClick={() => console.log(item.id)}
                                        >
                                            Szczegóły
                                        </button>
                                    </td>
                                </tr>
                            );
                        })}
                    </tbody>
                </table>
            </div>

            <style>{`
                .table-container {
                    padding: 2rem;
                    background-color: #ffffff;
                    min-height: 100vh;
                    font-family: 'Inter', sans-serif;
                }

                .table-header {
                    display: flex;
                    justify-content: space-between;
                    align-items: center;
                    margin-bottom: 2rem;
                }

                .table-header h1 { font-size: 1.6rem; margin: 0; color: #1a202c; }
                .count-tag { font-size: 0.85rem; color: #718096; margin-left: 10px; font-weight: 500; }

                .table-actions { display: flex; gap: 12px; }

                .btn-refresh { background: #edf2f7; border: none; padding: 10px 18px; border-radius: 8px; cursor: pointer; color: #4a5568; font-weight: 600; }
                .btn-add-primary { background: #4e73df; border: none; padding: 10px 20px; border-radius: 8px; cursor: pointer; color: white; font-weight: 600; transition: 0.2s; }
                .btn-add-primary:hover { background: #2e59d9; }

                .responsive-table-wrapper {
                    border: 1px solid #e2e8f0;
                    border-radius: 12px;
                    overflow: hidden;
                    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
                }

                .achievement-table {
                    width: 100%;
                    border-collapse: collapse;
                    text-align: left;
                    font-size: 0.95rem;
                }

                .achievement-table thead {
                    background-color: #f8fafc;
                    border-bottom: 2px solid #e2e8f0;
                }

                .achievement-table th {
                    padding: 16px;
                    color: #4a5568;
                    font-weight: 700;
                    text-transform: uppercase;
                    font-size: 0.75rem;
                    letter-spacing: 0.05em;
                }

                .achievement-table td {
                    padding: 16px;
                    border-bottom: 1px solid #edf2f7;
                    color: #2d3748;
                    vertical-align: middle;
                }

                .achievement-table tbody tr:hover {
                    background-color: #f9fbff;
                }

                .cat-badge {
                    padding: 4px 10px;
                    border: 1px solid;
                    border-radius: 6px;
                    font-size: 0.75rem;
                    font-weight: 700;
                    background: white;
                }

                .col-date { font-weight: 500; color: #718096; width: 100px; }
                .col-name { font-weight: 600; width: 200px; }
                .col-desc { 
                    max-width: 400px;
                    overflow: hidden;
                    text-overflow: ellipsis;
                    white-space: nowrap;
                    color: #718096;
                }
                .col-emp code { background: #f1f5f9; padding: 2px 6px; border-radius: 4px; font-size: 0.85rem; }

                .btn-view {
                    background: none;
                    border: 1px solid #e2e8f0;
                    padding: 6px 12px;
                    border-radius: 6px;
                    cursor: pointer;
                    font-size: 0.85rem;
                    color: #4e73df;
                    transition: 0.2s;
                }
                .btn-view:hover { background: #4e73df; color: white; border-color: #4e73df; }

                .loader-table { padding: 100px; text-align: center; font-size: 1.2rem; color: #a0aec0; }
            `}</style>
        </div>
    );
};

export default AchievementList;
