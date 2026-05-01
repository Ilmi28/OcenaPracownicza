import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import axiosClient from "../services/axiosClient";

/** Typy */
export type AchievementModel = {
    name: string;
    description: string;
    date: string;
    employeeId: string;
    category: number;
    period: string;
    finalScore: string;
    achievementsSummary: string;
};

interface CurrentUser {
    id: string;
    firstName: string;
    lastName: string;
}

interface ApiResponse<T> {
    success: boolean;
    message: string;
    data: T;
}

type Props = {
    initialEmployeeId?: string;
    onSuccess?: () => void;
};

const KATEGORIE = [
    { id: 1, nazwa: "Sukces projektowy" },
    { id: 2, nazwa: "Rozwój techniczny" },
    { id: 3, nazwa: "Innowacja" },
    { id: 4, nazwa: "Mentoring" },
    { id: 5, nazwa: "Inne" },
];

const formatToLocal = (iso: string) => iso.slice(0, 16);

const AddAchievementForm: React.FC<Props> = ({
    initialEmployeeId = "",
    onSuccess,
}) => {
    const navigate = useNavigate();
    const [currentUser, setCurrentUser] = useState<CurrentUser | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [loadingUser, setLoadingUser] = useState(true);
    const [message, setMessage] = useState<{
        type: "success" | "error";
        text: string;
    } | null>(null);

    const [form, setForm] = useState<AchievementModel>({
        name: "",
        description: "",
        date: new Date().toISOString(),
        employeeId: initialEmployeeId,
        category: 1,
        period: "",
        finalScore: "",
        achievementsSummary: "",
    });

    useEffect(() => {
        const fetchCurrentUserInfo = async () => {
            try {
                const resp = await axiosClient.get<ApiResponse<CurrentUser>>("/employee/me");
                const userData = resp.data.data;
                
                setCurrentUser(userData);

                setForm((prev) => ({
                    ...prev,
                    employeeId: userData.id,
                }));
            } catch (err: any) {
                console.error("Błąd pobierania profilu użytkownika:", err);
                setMessage({ 
                    type: "error", 
                    text: "Nie udało się pobrać danych Twojego profilu." 
                });
            } finally {
                setLoadingUser(false);
            }
        };

        fetchCurrentUserInfo();
    }, []);

    const handleChange = (
        e: React.ChangeEvent<
            HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
        >,
    ) => {
        const { name, value } = e.target;
        setForm((prev) => ({
            ...prev,
            [name]:
                name === "category"
                    ? Number(value)
                    : name === "date"
                      ? new Date(value).toISOString()
                      : value,
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        
        if (!form.employeeId) {
            setMessage({ type: "error", text: "Błąd: Brak przypisanego identyfikatora pracownika." });
            return;
        }

        setIsSubmitting(true);
        setMessage(null);

        try {
            await axiosClient.post("/achievement", form);
            if (onSuccess) onSuccess();
            navigate("/achievements");
        } catch (error: any) {
            const errorMsg =
                error.response?.data?.message ||
                error.message ||
                "Nie udało się zapisać osiągnięcia.";
            setMessage({ type: "error", text: errorMsg });
            setIsSubmitting(false);
        }
    };

    return (
        <form onSubmit={handleSubmit} className="achievement-form">
            <div className="form-header">
                <button
                    type="button"
                    onClick={() => navigate(-1)}
                    className="back-btn"
                >
                    ← Wróć
                </button>
                <h2>Dodaj Osiągnięcie</h2>
            </div>

            {message && (
                <div className={`alert ${message.type}`}>{message.text}</div>
            )}

            <div className="form-group">
                {loadingUser ? (
                    <div className="loading-placeholder">Pobieranie danych profilu...</div>
                ) : (
                    <div className="logged-user-info">
                        <span className="user-name">
                            {currentUser ? `${currentUser.firstName} ${currentUser.lastName}` : "Błąd! Zaloguj się ponownie!"}
                        </span>
                        <input type="hidden" name="employeeId" value={form.employeeId} />
                    </div>
                )}
            </div>

            {/* Reszta pól formularza pozostaje bez zmian */}
            <div className="form-group">
                <label htmlFor="name">Nazwa osiągnięcia</label>
                <input
                    id="name"
                    name="name"
                    type="text"
                    value={form.name}
                    onChange={handleChange}
                    required
                />
            </div>

            <div className="form-group">
                <label htmlFor="description">Opis</label>
                <textarea
                    id="description"
                    name="description"
                    value={form.description}
                    onChange={handleChange}
                    required
                    rows={4}
                />
            </div>

            <div className="form-group">
                <label htmlFor="date">Data i godzina</label>
                <input
                    id="date"
                    name="date"
                    type="datetime-local"
                    value={formatToLocal(form.date)}
                    onChange={handleChange}
                    required
                />
            </div>

            <div className="form-group">
                <label htmlFor="category">Kategoria</label>
                <select
                    id="category"
                    name="category"
                    value={form.category}
                    onChange={handleChange}
                >
                    {KATEGORIE.map((kat) => (
                        <option key={kat.id} value={kat.id}>
                            {kat.nazwa}
                        </option>
                    ))}
                </select>
            </div>

            <div className="form-group">
                <label htmlFor="period">Okres oceny</label>
                <input
                    id="period"
                    name="period"
                    type="text"
                    value={form.period}
                    onChange={handleChange}
                    required
                />
            </div>

            <div className="form-group">
                <label htmlFor="finalScore">Wynik końcowy</label>
                <input
                    id="finalScore"
                    name="finalScore"
                    type="text"
                    value={form.finalScore}
                    onChange={handleChange}
                    required
                />
            </div>

            <div className="form-group">
                <label htmlFor="achievementsSummary">Podsumowanie osiągnięć</label>
                <textarea
                    id="achievementsSummary"
                    name="achievementsSummary"
                    value={form.achievementsSummary}
                    onChange={handleChange}
                    required
                    rows={3}
                />
            </div>

            <button
                type="submit"
                className="submit-btn"
                disabled={isSubmitting || loadingUser}
            >
                {isSubmitting ? "Zapisywanie..." : "Zapisz i wróć do listy"}
            </button>

            <style>{`
                /* Style pozostają takie same jak w Twoim poprzednim fragmencie */
                .achievement-form {
                    max-width: 550px;
                    margin: 2rem auto;
                    display: grid;
                    gap: 1.25rem;
                    padding: 2.5rem;
                    background: #ffffff;
                    border-radius: 12px;
                    box-shadow: 0 10px 25px rgba(0,0,0,0.05);
                    font-family: 'Inter', sans-serif;
                }
                .form-header { display: flex; align-items: center; gap: 1rem; margin-bottom: 0.5rem; }
                .back-btn { background: none; border: none; color: #4e73df; cursor: pointer; font-weight: 600; }
                .form-group { display: flex; flex-direction: column; gap: 0.4rem; }
                label { font-size: 0.85rem; font-weight: 700; color: #4e5d78; }
                input, textarea, select { 
                    padding: 12px; border: 1px solid #dce1e8; border-radius: 8px; 
                    background-color: #f9fbff; color: #2e3b4e; font-size: 0.95rem;
                }
                .logged-user-info {
                    padding: 12px;
                    background-color: #edf2f7;
                    border: 1px solid #e2e8f0;
                    border-radius: 8px;
                }
                .user-name { font-weight: 600; color: #2d3748; }
                .submit-btn { 
                    margin-top: 1rem; padding: 14px; cursor: pointer; background: #4e73df; 
                    color: white; border: none; border-radius: 8px; font-weight: 700; font-size: 1rem;
                }
                .alert.error { background: #fed7d7; color: #822727; padding: 10px; border-radius: 5px; }
            `}</style>
        </form>
    );
};

export default AddAchievementForm;