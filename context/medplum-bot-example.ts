import { BotEvent, MedplumClient } from '@medplum/core';
import { Patient } from '@medplum/fhirtypes';

/**
 * Example Medplum bot that processes patient data
 */
export default async function handler(
  medplum: MedplumClient,
  event: BotEvent
): Promise<any> {
  try {
    // Validate input
    const input = event.input as Patient;

    if (!input || input.resourceType !== 'Patient') {
      return {
        success: false,
        error: 'Invalid input: expected Patient resource'
      };
    }

    // Log the operation
    console.log(`Processing patient: ${input.id}`);

    // Example: Update patient with additional metadata
    const updatedPatient = await medplum.updateResource({
      ...input,
      meta: {
        ...input.meta,
        tag: [
          ...(input.meta?.tag || []),
          {
            system: 'http://example.org/tags',
            code: 'processed',
            display: 'Processed by bot'
          }
        ]
      }
    });

    // Return success result
    return {
      success: true,
      patient: updatedPatient,
      message: 'Patient processed successfully'
    };

  } catch (error) {
    console.error('Error processing patient:', error);
    return {
      success: false,
      error: error instanceof Error ? error.message : 'Unknown error'
    };
  }
}
