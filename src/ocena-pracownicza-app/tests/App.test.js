import { jsx as _jsx } from "react/jsx-runtime";
import { describe, it, expect } from 'vitest';
import { render } from '@testing-library/react';
import App from '../src/App';
describe('App component', () => {
    it('renders without crashing', () => {
        const { container } = render(_jsx(App, {}));
        expect(container).toBeTruthy();
    });
});
